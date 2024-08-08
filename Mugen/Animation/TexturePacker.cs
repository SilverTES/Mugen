using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mugen.Animation
{
    [XmlRoot(ElementName = "sprite")]
    public class TexturePackerFrame
    {
        [XmlAttribute(AttributeName = "n")]
        public string? Name { get; set; }
        [XmlAttribute(AttributeName = "x")]
        public string? X { get; set; }
        [XmlAttribute(AttributeName = "y")]
        public string? Y { get; set; }
        [XmlAttribute(AttributeName = "w")]
        public string? W { get; set; }
        [XmlAttribute(AttributeName = "h")]
        public string? H { get; set; }
        [XmlAttribute(AttributeName = "pX")]
        public string? PX { get; set; }
        [XmlAttribute(AttributeName = "pY")]
        public string? PY { get; set; }
        [XmlAttribute(AttributeName = "oX")]
        public string? OX { get; set; }
        [XmlAttribute(AttributeName = "oY")]
        public string? OY { get; set; }
        [XmlAttribute(AttributeName = "oW")]
        public string? OW { get; set; }
        [XmlAttribute(AttributeName = "oH")]
        public string? OH { get; set; }
    }

    [XmlRoot(ElementName = "TextureAtlas")]
    public class TexturePackerAtlas
    {
        [XmlElement(ElementName = "sprite")]
        public List<TexturePackerFrame>? Frames { get; set; }
        [XmlAttribute(AttributeName = "imagePath")]
        public string? ImagePath { get; set; }
        [XmlAttribute(AttributeName = "width")]
        public string? Width { get; set; }
        [XmlAttribute(AttributeName = "height")]
        public string? Height { get; set; }

        [XmlIgnore]
        public Texture2D? _tex_Atlas = null;
    }

    public static class TexturePacker
    {
        public static Sprite LoadSpriteFromFile(ContentManager content, string fileXML, string filePNG = "")
        {
            Sprite sprite = new();

            TexturePackerAtlas texturePackerAtlas = IO.File.XmlSerialization.ReadFromXmlFile<TexturePackerAtlas>(fileXML)!;

            string? texAtlasName = Path.GetFileNameWithoutExtension(texturePackerAtlas.ImagePath);

            //string currentDirectory = Path.GetDirectoryName(fileXML);
            string? lastFolderName = Path.GetFileName(Path.GetDirectoryName(fileXML));

            if (filePNG == "")
                texturePackerAtlas._tex_Atlas = content.Load<Texture2D>(lastFolderName + "/" + texAtlasName);
            else
                texturePackerAtlas._tex_Atlas = content.Load<Texture2D>(filePNG);


            Animation? currentAnimation = null;

            string currentAnimationName = "";
            int index = 0;

            foreach (var tpaFrame in texturePackerAtlas.Frames!)
            {
                //Console.WriteLine("Frame Name = "+ item.Name);

                string name = tpaFrame.Name!.Split('0')[0];

                if (currentAnimationName != name) // Detect if change animation sequence 
                {

                    index = 0;
                    currentAnimationName = name;
                    //Console.WriteLine("Current Name = " + currentAnimationName);

                    // CreateAnimation 
                    currentAnimation = new Animation(texturePackerAtlas._tex_Atlas, currentAnimationName).SetLoop(Loops.REPEAT);
                    sprite.Add(currentAnimation);
                    //Console.WriteLine("Add Animation to Sprite : " + name);

                }

                if (currentAnimation != null)
                {
                    // Add Frame to current animation

                    int X = int.Parse(tpaFrame.X!);
                    int Y = int.Parse(tpaFrame.Y!);

                    int W = int.Parse(tpaFrame.W!);
                    int H = int.Parse(tpaFrame.H!);

                    float PX = float.Parse(tpaFrame.PX!.Replace('.', ','));
                    float PY = float.Parse(tpaFrame.PY!.Replace('.', ','));

                    //float OX = float.Parse(tpaFrame.OX);
                    //float OY = float.Parse(tpaFrame.OY);


                    Vector2 origin = new Vector2(PX * W, PY * H);

                    //Console.WriteLine("Pivot X,Y : " + origin.X + ", " + origin.Y);

                    Rectangle srcRect = new Rectangle(X, Y, W, H);

                    currentAnimation.Add(new Frame(srcRect, 1, origin.X, origin.Y));
                }

                //Console.WriteLine("Generate Name = " + currentAnimationName + index);
                index++;

            }

            return sprite;
        }
    }
}
