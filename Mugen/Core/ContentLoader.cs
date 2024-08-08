using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Xml.Serialization;

namespace Mugen.Core
{
    public class ContentLoader
    {
        static ContentRoot _contentRoot = new ContentRoot();

        public static Dictionary<string, SpriteFont> SpriteFonts = new Dictionary<string, SpriteFont>();
        public static Dictionary<string, Texture2D> Texture2Ds = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>();
        public static Dictionary<string, Song> Songs = new Dictionary<string, Song>();

        public static void AddContent(ContentManager content, string fileName)
        {
            ContentRoot contentRoot = IO.File.XmlSerialization.ReadFromXmlFile<ContentRoot>(fileName)!;

            foreach (var asset in contentRoot.SpriteFontLoaders)
            {
                // OverWrite 
                if (SpriteFonts.ContainsKey(asset.Name!)) SpriteFonts.Remove(asset.Name!);
                if (_contentRoot.SpriteFontLoaders.Find(x => x.Name == asset.Name) != null)
                    _contentRoot.SpriteFontLoaders.Remove(_contentRoot.SpriteFontLoaders.Find(x => x.Name == asset.Name)!);

                _contentRoot.SpriteFontLoaders.Add(asset);
                SpriteFonts[asset.Name!] = content.Load<SpriteFont>(asset.Path);
            }


            foreach (var asset in contentRoot.Texture2DLoaders)
            {
                // OverWrite 
                if (Texture2Ds.ContainsKey(asset.Name!)) Texture2Ds.Remove(asset.Name!);
                if (_contentRoot.Texture2DLoaders.Find(x => x.Name == asset.Name) != null)
                    _contentRoot.Texture2DLoaders.Remove(_contentRoot.Texture2DLoaders.Find(x => x.Name == asset.Name)!);

                _contentRoot.Texture2DLoaders.Add(asset);
                Texture2Ds[asset.Name!] = content.Load<Texture2D>(asset.Path);
            }

            foreach (var asset in contentRoot.SoundEffectLoaders)
            {
                // OverWrite 
                if (SoundEffects.ContainsKey(asset.Name!)) SoundEffects.Remove(asset.Name!);
                if (_contentRoot.SoundEffectLoaders.Find(x => x.Name == asset.Name) != null)
                    _contentRoot.SoundEffectLoaders.Remove(_contentRoot.SoundEffectLoaders.Find(x => x.Name == asset.Name)!);

                _contentRoot.SoundEffectLoaders.Add(asset);
                SoundEffects[asset.Name!] = content.Load<SoundEffect>(asset.Path);
            }

            foreach (var asset in contentRoot.SongLoaders)
            {
                // OverWrite 
                if (Songs.ContainsKey(asset.Name!)) Songs.Remove(asset.Name!);
                if (_contentRoot.SongLoaders.Find(x => x.Name == asset.Name) != null)
                    _contentRoot.SongLoaders.Remove(_contentRoot.SongLoaders.Find(x => x.Name == asset.Name)!);

                _contentRoot.SongLoaders.Add(asset);
                Songs[asset.Name!] = content.Load<Song>(asset.Path);
            }

            //if (null != contentRoot.ImportStyleLoaders)
            //if (contentRoot.ImportStyleLoaders.Count == 0)
            //{
            //	Console.ForegroundColor = ConsoleColor.DarkYellow;
            //	Console.WriteLine($"Warning ContentLoader : Styles is Empty ! {contentRoot.ImportStyleLoaders.Count}");
            //	Console.ResetColor();
            //}

            foreach (var styleSrc in contentRoot.ImportStyleLoaders)
            {
                //if (styleSrc != null)
                if (!StyleContainer.ImportStyleFromFile(styleSrc.src!))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error ContentLoader : failed to load Style | {styleSrc.src}");
                    Console.ResetColor();
                }
            }

        }

        public static void LogAllStyle()
        {
            StyleContainer.LogAll();
        }
        public static void LogAllSpriteFont()
        {
            foreach (var asset in _contentRoot.SpriteFontLoaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("SpriteFont : " + asset.Name + " -> " + asset.Path);
                Console.ResetColor();
            }
        }
        public static void LogAllTexture2D()
        {
            foreach (var asset in _contentRoot.Texture2DLoaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Texture2D : " + asset.Name + " -> " + asset.Path);
                Console.ResetColor();
            }
        }
        public static void LogAllSoundEffect()
        {
            foreach (var asset in _contentRoot.SoundEffectLoaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("SoundEffect : " + asset.Name + " -> " + asset.Path);
                Console.ResetColor();
            }
        }
        public static void LogAllSong()
        {
            foreach (var asset in _contentRoot.SongLoaders)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Song : " + asset.Name + " -> " + asset.Path);
                Console.ResetColor();
            }
        }
        public static void LogAll()
        {
            LogAllStyle();
            LogAllSpriteFont();
            LogAllTexture2D();
            LogAllSoundEffect();
            LogAllSong();
        }

        [XmlRoot(ElementName = "ImportStyle")]
        public class ImportStyle
        {
            [XmlAttribute(AttributeName = "src")]
            public string? src { get; set; }

        }

        [XmlRoot(ElementName = "SpriteFont")]
        public class SpriteFontLoader
        {
            [XmlAttribute(AttributeName = "name")]
            public string? Name { get; set; }
            [XmlAttribute(AttributeName = "path")]
            public string? Path { get; set; }
        }

        [XmlRoot(ElementName = "Texture2D")]
        public class Texture2DLoader
        {
            [XmlAttribute(AttributeName = "name")]
            public string? Name { get; set; }
            [XmlAttribute(AttributeName = "path")]
            public string? Path { get; set; }
        }

        [XmlRoot(ElementName = "SoundEffect")]
        public class SoundEffectLoader
        {
            [XmlAttribute(AttributeName = "name")]
            public string? Name { get; set; }
            [XmlAttribute(AttributeName = "path")]
            public string? Path { get; set; }
        }

        [XmlRoot(ElementName = "Song")]
        public class SongLoader
        {
            [XmlAttribute(AttributeName = "name")]
            public string? Name { get; set; }
            [XmlAttribute(AttributeName = "path")]
            public string? Path { get; set; }
        }

        [XmlRoot(ElementName = "Content")]
        public class ContentRoot
        {
            [XmlElement(ElementName = "ImportStyle")]
            public List<ImportStyle> ImportStyleLoaders { get; set; } = new List<ImportStyle>();
            [XmlElement(ElementName = "SpriteFont")]
            public List<SpriteFontLoader> SpriteFontLoaders { get; set; } = new List<SpriteFontLoader>();
            [XmlElement(ElementName = "Texture2D")]
            public List<Texture2DLoader> Texture2DLoaders { get; set; } = new List<Texture2DLoader>();
            [XmlElement(ElementName = "SoundEffect")]
            public List<SoundEffectLoader> SoundEffectLoaders { get; set; } = new List<SoundEffectLoader>();
            [XmlElement(ElementName = "Song")]
            public List<SongLoader> SongLoaders { get; set; } = new List<SongLoader>();
        }

    }
}
