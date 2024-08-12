using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.GUI;
using System.Reflection;

namespace Mugen.Core
{
    public class StyleContainer
    {
        static Dictionary<string, Style> _styles = new Dictionary<string, Style>();

        public static bool Import(string styleName, ref Style style, SpriteFont font)
        {
            if (_styles.ContainsKey(styleName))
            {
                style = _styles[styleName];
                return true;
            }
            else
            {
                style = new Style(font);
                return false;
            }
        }


        public static Style PreProcess(Style containerStyle)
        {
            // Check all ColorValue in json, if name != "" replace _value by ColorName
            var props = containerStyle.GetType().GetProperties();

            foreach (var p in props)
            {
                //var propsValue = p.GetValue(style).GetType().GetProperties();
                var prop = p.GetValue(containerStyle)!.GetType();
                //Console.WriteLine("Property Name = " + propName);

                // if Property type is ColorValue !
                if (prop.Name == "ColorValue")
                {
                    PropertyInfo value = p.GetValue(containerStyle)!.GetType().GetProperty("_value")!;
                    Color valueValue = (Color)value!.GetValue(p.GetValue(containerStyle))!;

                    PropertyInfo name = p.GetValue(containerStyle)!.GetType().GetProperty("_name")!;
                    string nameValue = name.GetValue(p.GetValue(containerStyle))!.ToString()!;

                    //Console.WriteLine("name = " + nameValue);
                    //Console.WriteLine("value = " + valueValue);

                    if (nameValue != "")
                    {
                        // Les 2 solutions fonctionnent : soit on modifie l'attribut <Color.Value._value> soit tout l'objet <ColorValue>
                        value.SetValue(p.GetValue(containerStyle), Style.ColorValue.GetColor(nameValue));
                        //p.SetValue(style, Style.ColorValue.MakeColor(nameValue));

                        //Console.WriteLine("Convert String ColorName to Color");
                    }

                }
            }

            if (containerStyle._fontName != "")
            {
                if (ContentLoader.SpriteFonts.ContainsKey(containerStyle._fontName))
                    containerStyle._font = ContentLoader.SpriteFonts[containerStyle._fontName];
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("fontName not exist in ContentLoader");
                    Console.ResetColor();
                }
            }

            return containerStyle;
        }

        //public static Style ImportStyleFromContainer(string styleName, ref bool success)
        //{
        //    Style containerStyle = new Style();

        //    success = Import(styleName, ref containerStyle);

        //    if (success)
        //    {

        //        containerStyle = PreProcess(containerStyle);

        //        //// Check all ColorValue in json, if name != "" replace _value by ColorName
        //        //var props = containerStyle.GetType().GetProperties();

        //        //foreach (var p in props)
        //        //{
        //        //    //var propsValue = p.GetValue(style).GetType().GetProperties();
        //        //    var prop = p.GetValue(containerStyle).GetType();
        //        //    //Console.WriteLine("Property Name = " + propName);

        //        //    // if Property type is ColorValue !
        //        //    if (prop.Name == "ColorValue")
        //        //    {
        //        //        PropertyInfo value = p.GetValue(containerStyle).GetType().GetProperty("_value");
        //        //        Color valueValue = (Color)value.GetValue(p.GetValue(containerStyle));

        //        //        PropertyInfo name = p.GetValue(containerStyle).GetType().GetProperty("_name");
        //        //        string nameValue = name.GetValue(p.GetValue(containerStyle)).ToString();

        //        //        //Console.WriteLine("name = " + nameValue);
        //        //        //Console.WriteLine("value = " + valueValue);

        //        //        if (nameValue != "")
        //        //        {
        //        //            // Les 2 solutions fonctionnent : soit on modifie l'attribut <Color.Value._value> soit tout l'objet <ColorValue>
        //        //            value.SetValue(p.GetValue(containerStyle), Style.ColorValue.GetColor(nameValue));
        //        //            //p.SetValue(style, Style.ColorValue.MakeColor(nameValue));

        //        //            //Console.WriteLine("Convert String ColorName to Color");
        //        //        }

        //        //    }

        //        //}

        //        //if (containerStyle._fontName != "")
        //        //{
        //        //    if (ContentLoader.SpriteFonts.ContainsKey(containerStyle._fontName))
        //        //        containerStyle._font = ContentLoader.SpriteFonts[containerStyle._fontName];
        //        //    else
        //        //        Console.WriteLine("fontName not exist in ContentLoader");
        //        //}


        //    }
        //    else
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("Can't Import Style !");
        //        Console.ResetColor();
        //    }

        //    return containerStyle;
        //}
        public static bool ImportStyleFromFile(string fileName)
        {
            bool success = true;
            Dictionary<string, Style> styles = IO.File.JsonSerialization.ReadFromJsonFile<Dictionary<string, Style>>(fileName, ref success)!;
            //_styles = FileIO.JsonSerialization.ReadFromJsonFile<Dictionary<string, Style>>(fileName, ref success);

            //Console.WriteLine($"------- ImportStyleFromFile {fileName}");

            if (success)
            {
                //_styles = styles;
                if (null != styles)
                {
                    if (styles.Count > 0)
                        foreach (var style in styles)
                        {
                            if (null != style.Value)
                            {
                                //Console.WriteLine("style.Key : " + style.Key);

                                style.Value._name = fileName;

                                // if exist then overwrite : Remove Old & Add New
                                if (_styles.ContainsKey(style.Key))
                                    _styles[style.Key] = PreProcess(style.Value);
                                else
                                    _styles.Add(style.Key, PreProcess(style.Value));
                            }
                        }

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"SUCCESS StyleContainer : ImportStyleFromFile OK | {fileName}  Styles.Count = {styles.Count}");
                    Console.ResetColor();
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"ERROR StyleContainer : ImportStyleFromFile FAILED | {fileName}");
                Console.ResetColor();
            }

            return success;
        }

        public static void LogAll()
        {
            if (_styles.Count > 0)
                foreach (var style in _styles)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Style {0} <---- {1} ", style.Key, style.Value._name);
                    Console.ResetColor();
                }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("WARNING StyleContainer : StyleContainer is Empty !");
                Console.ResetColor();
            }
        }
    }
}
