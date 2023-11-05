using System;
using System.IO;

namespace Game.Client
{
    public static class VRAssetPath
    {
        public const string JPG = ".jpg";
        public const string JPEG = ".jpeg";
        public const string PNG = ".png";
        public const string BMP = ".bmp";

        public const string MP4 = ".mp4";
        public const string MOV = ".mov";

        public const string WAV = ".wav";
        public const string MP3 = ".mp3";

        public const string PDF = ".pdf";

        //// "https://ricardoreis.net/trilib-accepted-file-formats/"
        public const string FBX = ".fbx";
        public const string OBJ = ".obj";
        //public const string GLTF2 = ".gltf";
        public const string STL = ".stl";
        //public const string PLY = ".ply";
        public const string M_3DS = ".3ds";
        public const string ZIP = ".zip";
        public const string IFC = ".ifc";

        public static VrAssetType GetTypeAsset(string fileName)
        {
            var extension = Path.GetExtension(fileName);

            if (extension == null) return VrAssetType.Unknown;

            if (extension.Equals(JPG, StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(JPEG, StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(PNG, StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(BMP, StringComparison.OrdinalIgnoreCase))
            {
                return VrAssetType.Image;
            }
            else if (extension.Equals(MP4, StringComparison.OrdinalIgnoreCase) ||
                        extension.Equals(MOV, StringComparison.OrdinalIgnoreCase))
            {
                return VrAssetType.Video;
            }
            else if (extension.Equals(MP3, StringComparison.OrdinalIgnoreCase) ||
                        extension.Equals(WAV, StringComparison.OrdinalIgnoreCase))
            {
                return VrAssetType.Sound;
            }
            else if (extension.Equals(PDF, StringComparison.OrdinalIgnoreCase))
            {
                return VrAssetType.Pdf;
            }
            else if (extension.Equals(FBX, StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(OBJ, StringComparison.OrdinalIgnoreCase) ||
                    //extension.Equals(GLTF2, StringComparison.OrdinalIgnoreCase) ||
                    //extension.Equals(PLY, StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(M_3DS, StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(ZIP, StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(IFC, StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(STL, StringComparison.OrdinalIgnoreCase))
            {
                return VrAssetType.Model;
            }

            return VrAssetType.Unknown;
        }
    }
}