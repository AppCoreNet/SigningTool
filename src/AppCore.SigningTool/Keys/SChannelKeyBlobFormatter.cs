using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace AppCore.SigningTool.Keys
{
    internal class SChannelKeyBlobFormatter
    {
        const uint CALG_RSA_SIGN = 0x00002400;
        const uint RSA2_SIG = 0x32415352;
        const uint RSA1_SIG = 0x31415352;

        public static void Serialize(Stream stream, RsaKeyPair keyPair)
        {
            RsaPrivateKey privateKey = keyPair.PrivateKey;
            if (privateKey != null)
            {
                using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
                SerializePrivateKey(writer, privateKey);
            }
            else
            {
                Serialize(stream, keyPair.PublicKey);
            }
        }

        public static void Serialize(Stream stream, RsaPublicKey publicKey)
        {
        }

        private static void SerializePrivateKey(BinaryWriter writer, RsaPrivateKey privateKey)
        {
            writer.Write((byte)7);          // bType (public/private key)
            writer.Write((byte)2);          // bVersion
            writer.Write((ushort)0);        // reserved
            writer.Write(CALG_RSA_SIGN);    // aiKeyAlg
            writer.Write(RSA2_SIG);         // magic (RSA2)
            writer.Write(privateKey.Modulus.Length * 8);   // bitlen
            writer.WriteReverse(privateKey.PublicExponent);
            writer.Write(new byte[4 - privateKey.PublicExponent.Length]); // pad to DWORD
            writer.WriteReverse(privateKey.Modulus);
            writer.WriteReverse(privateKey.Prime1);
            writer.WriteReverse(privateKey.Prime2);
            writer.WriteReverse(privateKey.Exponent1);
            writer.WriteReverse(privateKey.Exponent2);
            writer.WriteReverse(privateKey.Coefficient);
            writer.WriteReverse(privateKey.PrivateExponent);
        }

        public static RsaKeyPair Deserialize(Stream stream)
        {
            RsaKeyPair keyPair;
            try
            {
                keyPair = DeserializeKeyPair(new BinaryReader(stream, Encoding.UTF8, true));
            }
            catch (InvalidKeyException)
            {
                stream.Seek(0, SeekOrigin.Begin);
                keyPair = new RsaKeyPair(null, DeserializePublicKey(new BinaryReader(stream, Encoding.UTF8, true)));
            }

            return keyPair;
        }

        private static RsaKeyPair DeserializeKeyPair(BinaryReader reader)
        {
            try
            {
                // Read PUBLICKEYSTRUC
                if (reader.ReadByte() != 7)
                    throw new InvalidKeyException("Not a public/private key pair blob.");

                if (reader.ReadByte() != 2)
                    throw new InvalidKeyException("Invalid key blob version.");

                reader.ReadUInt16();    // reserved
                if (reader.ReadUInt32() != CALG_RSA_SIGN)
                    throw new InvalidKeyException("Not a RSA signing key blob.");

                // Read RSAPUBKEY
                if (reader.ReadUInt32() != RSA2_SIG)    // magic = RSA2
                    throw new InvalidKeyException("Invalid RSA2 signature magic.");

                uint bitLength = reader.ReadUInt32();
                byte[] publicExponent = reader.ReadBytesReverse(4);

                int len8 = (int)(bitLength / 8);
                int len16 = (int)(bitLength / 16);

                // Read the rest
                byte[] modulus = reader.ReadBytesReverse(len8);
                byte[] prime1 = reader.ReadBytesReverse(len16);
                byte[] prime2 = reader.ReadBytesReverse(len16);
                byte[] exponent1 = reader.ReadBytesReverse(len16);
                byte[] exponent2 = reader.ReadBytesReverse(len16);
                byte[] coefficient = reader.ReadBytesReverse(len16);
                byte[] privateExponent = reader.ReadBytesReverse(len8);

                var privateKey = new RsaPrivateKey(
                    publicExponent,
                    modulus,
                    prime1,
                    prime2,
                    exponent1,
                    exponent2,
                    coefficient,
                    privateExponent);

                var publicKey = new RsaPublicKey(modulus, publicExponent, AssemblyHashAlgorithm.Sha1);

                return new RsaKeyPair(privateKey, publicKey);
            }
            catch (IOException ex)
            {
                throw new InvalidKeyException("Couldn't read key blob.", ex);
            }
        }

        private static RsaPublicKey DeserializePublicKey(BinaryReader reader)
        {
            try
            {
                // Read PublicKeyBlob
                uint signatureAlgorithm = reader.ReadUInt32();
                var hashAlgorithm = (AssemblyHashAlgorithm)reader.ReadUInt32();
                /*int pkLen = */
                reader.ReadInt32();

                // Read PUBLICKEYSTRUC
                if (reader.ReadByte() != 6)
                    throw new InvalidKeyException("Not a public key blob.");

                if (reader.ReadByte() != 2)
                    throw new InvalidKeyException("Invalid key blob version.");

                reader.ReadUInt16();    // reserved

                if (reader.ReadUInt32() != CALG_RSA_SIGN)
                    throw new InvalidKeyException("Not a RSA signing key blob.");

                // Read RSAPUBKEY
                if (reader.ReadUInt32() != RSA1_SIG)    // magic = RSA1
                    throw new InvalidKeyException("Invalid RSA1 signature magic.");

                uint bitLength = reader.ReadUInt32();
                byte[] publicExponent = reader.ReadBytesReverse(4);
                byte[] modulus = reader.ReadBytesReverse((int)(bitLength / 8));

                return new RsaPublicKey(modulus, publicExponent, hashAlgorithm);
            }
            catch (IOException ex)
            {
                throw new InvalidKeyException("Couldn't read key blob.", ex);
            }
        }
    }
}