using System;
using System.IO;

namespace UnityRipper.Bundles
{
	internal class BundleFile
	{
		public static bool IsBundleFile(string bundlePath)
		{
			if (!File.Exists(bundlePath))
			{
				throw new Exception($"Bundle at path '{bundlePath}' doesn't exist");
			}

			using (FileStream fileStream = File.OpenRead(bundlePath))
			{
				using (EndianStream stream = new EndianStream(fileStream, EndianType.BigEndian))
				{
					bool isBundle = true;
					try
					{
						string signatureStr = stream.ReadStringZeroTerm();
						BundleSignature signature = StringToSignature(signatureStr);
					}
					catch
					{
						isBundle = false;
					}

					return isBundle;
				}
			}
		}

		public AssetsFileData Load(string bundlePath)
		{
			if(!File.Exists(bundlePath))
			{
				throw new Exception($"Bundle at path '{bundlePath}' doesn't exist");
			}

			FileStream stream = null;
			try
			{
				stream = File.OpenRead(bundlePath);
				Parse(stream, true);
			}
			finally
			{
				if(m_isNewDataStream)
				{
					stream.Dispose();
				}
			}
			return m_fileData;
		}

		public AssetsFileData Parse(Stream baseStream)
		{
			Parse(baseStream, false);
			return m_fileData;
		}

		private void Parse(Stream baseStream, bool isNewStream)
		{
			m_isNewStream = isNewStream;
			using (EndianStream stream = new EndianStream(baseStream, EndianType.BigEndian))
			{
				string signatureStr = stream.ReadStringZeroTerm();
				Signature = StringToSignature(signatureStr);
				Format = (BundleFormat)stream.ReadInt32();
				VersionPlayer = stream.ReadStringZeroTerm();
				VersionEngine = stream.ReadStringZeroTerm();

				switch (Signature)
				{
					case BundleSignature.UnityRaw:
					case BundleSignature.UnityWeb:
					case BundleSignature.HexFA:
						ParseBundleLoading(stream);
						break;

					case BundleSignature.UnityFS:
						ParseBundleStreaming(stream);
						break;

					default:
						throw new Exception($"Unknown bundle signature '{Signature}'");
				}
			}
		}

		private void ParseBundleLoading(EndianStream stream)
		{
			if(IsBundleFormat)
			{
				ParseBundleFormat(stream);
			}
			else if (IsBundleFormat6)
			{
				ParseBundleFormat6(stream, true);
			}
			else
			{
				throw new NotSupportedException($"Unsupported format {Format}");
			}
		}

		private void ParseBundleStreaming(EndianStream stream)
		{
			if (IsBundleFormat6)
			{
				ParseBundleFormat6(stream, false);
			}
			else
			{
				throw new NotImplementedException($"Streaming asset bundle with format {Format}");
			}
		}

		private void ParseBundleFormat(EndianStream stream)
		{
			int bundleSize = stream.ReadInt32();

			short unused16 = stream.ReadInt16();
			int offset = stream.ReadInt16();
			int unused32 = stream.ReadInt32();
			int lzmaChunkCount = stream.ReadInt32();

			if (lzmaChunkCount != 1)
			{
				// see original project, it support only 1 chunk
				throw new Exception($"Found lzma with {lzmaChunkCount} chunks");
			}
			
			MemoryStream memStream = IsBundlePacked ? new MemoryStream() : null;
			for (int i = 0; i < lzmaChunkCount; i++)
			{
				int lzmaSize = stream.ReadInt32();
				int decompressedSize = stream.ReadInt32();

				stream.BaseStream.Position = offset;
				switch (Signature)
				{
					case BundleSignature.UnityRaw:
						ParseBundleFiles(stream);
						break;

					case BundleSignature.UnityWeb:
					case BundleSignature.HexFA:
						memStream.Position = 0;

						SevenZipHelper.DecompressLZMASizeStream(stream.BaseStream, lzmaSize, memStream);
						using (EndianStream decompressStream = new EndianStream(memStream, EndianType.BigEndian))
						{
							ParseBundleFiles(decompressStream);
						}
						break;

					default:
						throw new Exception($"Unsupported bundle signature '{Signature}'");
				}

				offset += lzmaSize;
			}
		}

		private void ParseBundleFormat6(EndianStream stream, bool isPadding)
		{
			long bundleSize = stream.ReadInt64();
			int compressSize = stream.ReadInt32();
			int decompressSize = stream.ReadInt32();
			int flag = stream.ReadInt32();
			if(isPadding)
			{
				stream.BaseStream.Position++;
			}

			long blockPosition;
			long dataPosition;
			int isDataFirst = flag & 0x80;
			if (isDataFirst != 0)
			{
				blockPosition = stream.BaseStream.Length - compressSize;
				dataPosition = stream.BaseStream.Position;
			}
			else
			{
				blockPosition = stream.BaseStream.Position;
				dataPosition = stream.BaseStream.Position + compressSize;
			}

			stream.BaseStream.Position = blockPosition;
			BundleCompressType compressType = (BundleCompressType)(flag & 0x3F);
			switch(compressType)
			{
				case BundleCompressType.None:
					ParseBundle6BlockInfo(stream, ref blockPosition, stream, ref dataPosition);
					ParseBundle6Files(stream, ref blockPosition, stream, ref dataPosition);
					break;

				case BundleCompressType.LZMA:
					using (MemoryStream memStream = SevenZipHelper.DecompressLZMASSizeStream(stream.BaseStream, compressSize))
					{
						using (EndianStream decBlockStream = new EndianStream(memStream, EndianType.BigEndian))
						{
							// update position acerding to newly created stream
							blockPosition = 0;
							using (EndianStream decDataStream = ParseBundle6BlockInfo(decBlockStream, ref blockPosition, stream, ref dataPosition))
							{
								ParseBundle6Files(decBlockStream, ref blockPosition, decDataStream, ref dataPosition);
							}
						}
					}
					break;

				case BundleCompressType.LZ4:
				case BundleCompressType.LZ4HZ:
					using (MemoryStream memStream = new MemoryStream(decompressSize))
					{
						using (Lz4Stream lzStream = new Lz4Stream(stream.BaseStream, compressSize))
						{
							lzStream.Read(memStream, decompressSize);
						}
						using (EndianStream decBlockStream = new EndianStream(memStream, EndianType.BigEndian))
						{
							// update position acerding to newly created stream
							blockPosition = 0;
							using (EndianStream decDataStream = ParseBundle6BlockInfo(decBlockStream, ref blockPosition, stream, ref dataPosition))
							{
								ParseBundle6Files(decBlockStream, ref blockPosition, decDataStream, ref dataPosition);
							}
						}
					}
					break;

				default:
					throw new NotImplementedException($"Bundle compression '{compressType}' isn't supported");
			}
		}

		/// <summary>
		/// Read block infos and create uncompressed stream with corresponding data
		/// </summary>
		/// <param name="blockStream">Stream with block infos</param>
		/// <param name="blockPosition">BlockInfos position within block stream</param>
		/// <param name="dataStream">Stream with compressed data</param>
		/// <param name="dataPosition">CompressedData position within data stream</param>
		/// <returns>Uncompressed data stream</returns>
		private EndianStream ParseBundle6BlockInfo(EndianStream blockStream, ref long blockPosition, EndianStream dataStream, ref long dataPosition)
		{
			// dataStream and blockStream could be same stream. so we should handle this situation properly
			
			MemoryStream memStream = null;
			EndianStream resultStream = null;
			long read = 0;

			blockStream.BaseStream.Position = blockPosition;
			blockStream.BaseStream.Position += 0x10;
			int blockCount = blockStream.ReadInt32();
			blockPosition = blockStream.BaseStream.Position;
			for (int i = 0; i < blockCount; i++)
			{
				// prepare block position
				blockStream.BaseStream.Position = blockPosition;

				int decompressSize = blockStream.ReadInt32();
				int compressSize = blockStream.ReadInt32();
				int flag = blockStream.ReadInt16();
				blockPosition = blockStream.BaseStream.Position;

				dataStream.BaseStream.Position = dataPosition;
				BundleCompressType compressType = (BundleCompressType)(flag & 0x3F);
				if(i == 0)
				{
					if(compressType == BundleCompressType.None)
					{
						resultStream = dataStream;
						m_isNewDataStream = false;
					}
					else
					{
						memStream = new MemoryStream();
						resultStream = new EndianStream(memStream, EndianType.BigEndian);
						m_isNewDataStream = true;
					}
				}
				else
				{
					if (compressType != BundleCompressType.None && !m_isNewDataStream)
					{
						// TODO: if first block is none compressed then we should create stream and copy all previous blocks into it 
						// but for now just throw exception
						throw new NotImplementedException("None compression");
					}
				}

				switch (compressType)
				{
					case BundleCompressType.None:
						if(m_isNewDataStream)
						{
							if(decompressSize != compressSize)
							{
								throw new Exception($"Compressed {compressSize} and decompressed {decompressSize} sizes differ");
							}

							dataStream.BaseStream.CopyStream(memStream, compressSize);
						}
						break;

					case BundleCompressType.LZMA:
						SevenZipHelper.DecompressLZMAStream(dataStream.BaseStream, compressSize, memStream, decompressSize);
						break;

					case BundleCompressType.LZ4:
					case BundleCompressType.LZ4HZ:
						using (Lz4Stream lzStream = new Lz4Stream(dataStream.BaseStream, compressSize))
						{
							lzStream.Read(memStream, decompressSize);
						}
						break;

					default:
						throw new NotImplementedException($"Bundle compression '{compressType}' isn't supported");
				}

				dataPosition += compressSize;
				read += compressSize;

				if (m_isNewDataStream)
				{
					if(dataPosition != dataStream.BaseStream.Position)
					{
						throw new Exception($"Read data length is differ from compressed size for {i}th block");
					}
				}
			}

			// update position acording to result stream
			if (m_isNewDataStream)
			{
				dataPosition = 0;
			}
			else
			{
				dataPosition -= read;
			}

			return resultStream;
		}

		private void ParseBundleFiles(EndianStream stream)
		{
			bool isClosable = m_isNewStream || m_isNewDataStream;
			m_fileData = new AssetsFileData(stream.BaseStream, isClosable);
			long baseOffset = stream.BaseStream.Position;
			int fileCount = stream.ReadInt32();
			for (int i = 0; i < fileCount; i++)
			{
				string fileName = stream.ReadStringZeroTerm();
				int fileOffset = stream.ReadInt32();
				int fileSize = stream.ReadInt32();

				long entryOffset = baseOffset + fileOffset;
				m_fileData.AddEntry(fileName, fileOffset, fileSize);
			}
		}

		private void ParseBundle6Files(EndianStream blockStream, ref long blockPosition, EndianStream dataStream, ref long dataPosition)
		{
			// blockStream and dataStream could be same stream. so we should handle this situation properly

			bool isClosable = m_isNewStream || m_isNewDataStream;
			m_fileData = new AssetsFileData(dataStream.BaseStream, isClosable);
			blockStream.BaseStream.Position = blockPosition;
			int fileCount = blockStream.ReadInt32();
			for (int i = 0; i < fileCount; i++)
			{
				long fileOffset = blockStream.ReadInt64();
				long fileSize = blockStream.ReadInt64();
				int unused = blockStream.ReadInt32();
				string fileName = blockStream.ReadStringZeroTerm();

				long entryOffset = dataPosition + fileOffset;
				m_fileData.AddEntry(fileName, entryOffset, fileSize);
			}
		}

		private static BundleSignature StringToSignature(string signatureString)
		{
			if(signatureString == BundleSignature.UnityWeb.ToString())
			{
				return BundleSignature.UnityWeb;
			}
			if(signatureString == BundleSignature.UnityRaw.ToString())
			{
				return BundleSignature.UnityRaw;
			}
			if(signatureString == BundleSignature.UnityFS.ToString())
			{
				return BundleSignature.UnityFS;
			}
			if(signatureString == HexFASignature)
			{
				return BundleSignature.HexFA;
			}

			throw new ArgumentException($"Unknown signature '{signatureString}'");
		}

		public string VersionPlayer { get; private set; } = string.Empty;
		public string VersionEngine { get; private set; } = string.Empty;

		private BundleSignature Signature { get; set; }
		private BundleFormat Format { get; set; }

		private bool IsBundleFormat => Format < BundleFormat.Format6;
		private bool IsBundleFormat6 => Format == BundleFormat.Format6;
		private bool IsBundlePacked => Signature == BundleSignature.UnityWeb || Signature == BundleSignature.HexFA;

		private const string HexFASignature = "\xFA\xFA\xFA\xFA\xFA\xFA\xFA\xFA";

#warning rewrite
		private bool m_isNewStream = false;
		private bool m_isNewDataStream = false;
		private AssetsFileData m_fileData = null;
	}
}
