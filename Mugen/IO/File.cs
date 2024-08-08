using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mugen.IO
{
    public static class File
    {
        public static Texture2D LoadTexture2DFromFile(GraphicsDevice graphicsDevice, string path)
        {
            Texture2D texture2D;
            FileStream fileStream = new FileStream(path, FileMode.Open);
            texture2D = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();
            return texture2D;
        }


        // BINARY SERIALIZATION

        /// <summary>
        /// Functions for performing common binary Serialization operations.
        /// <para>All properties and variables will be serialized.</para>
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        public static class BinarySerialization
        {
            /// <summary>
            /// Writes the given object instance to a binary file.
            /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
            /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
            /// </summary>
            /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
            /// <param name="filePath">The file path to write the object instance to.</param>
            /// <param name="objectToWrite">The object instance to write to the XML file.</param>
            /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
            public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
            {
                using (Stream stream = System.IO.File.Open(filePath, append ? FileMode.Append : FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, objectToWrite);
                }
            }

            /// <summary>
            /// Reads an object instance from a binary file.
            /// </summary>
            /// <typeparam name="T">The type of object to read from the XML.</typeparam>
            /// <param name="filePath">The file path to read the object instance from.</param>
            /// <returns>Returns a new instance of the object read from the binary file.</returns>
            public static T? ReadFromBinaryFile<T>(string filePath)
            {
                using (Stream stream = System.IO.File.Open(filePath, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (T)binaryFormatter.Deserialize(stream);
                }
            }
        }

        // XML SERIALIZATION

        /// <summary>
        /// Functions for performing common XML Serialization operations.
        /// <para>Only public properties and variables will be serialized.</para>
        /// <para>Use the [XmlIgnore] attribute to prevent a property/variable from being serialized.</para>
        /// <para>Object to be serialized must have a parameterless constructor.</para>
        /// </summary>
        public static class XmlSerialization
        {
            /// <summary>
            /// Writes the given object instance to an XML file.
            /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
            /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
            /// <para>Object type must have a parameterless constructor.</para>
            /// </summary>
            /// <typeparam name="T">The type of object being written to the file.</typeparam>
            /// <param name="filePath">The file path to write the object instance to.</param>
            /// <param name="objectToWrite">The object instance to write to the file.</param>
            /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
            public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
            {
                TextWriter? writer = null;
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    writer = new StreamWriter(filePath, append);
                    serializer.Serialize(writer, objectToWrite);
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }

            /// <summary>
            /// Reads an object instance from an XML file.
            /// <para>Object type must have a parameterless constructor.</para>
            /// </summary>
            /// <typeparam name="T">The type of object to read from the file.</typeparam>
            /// <param name="filePath">The file path to read the object instance from.</param>
            /// <returns>Returns a new instance of the object read from the XML file.</returns>
            public static T? ReadFromXmlFile<T>(string filePath) where T : new()
            {
                TextReader? reader = null;
                try
                {
                    var serializer = new XmlSerializer(typeof(T));
                    reader = new StreamReader(filePath);
                    return (T)serializer.Deserialize(reader)!;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR Read file '{0}' : {1} ", filePath, e.Message);
                    Console.ResetColor();

                    return default;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }

        // JSON SERIALIZATION

        /// <summary>
        /// Functions for performing common Json Serialization operations.
        /// <para>Requires the Newtonsoft.Json assembly (Json.Net package in NuGet Gallery) to be referenced in your project.</para>
        /// <para>Only public properties and variables will be serialized.</para>
        /// <para>Use the [JsonIgnore] attribute to ignore specific public properties or variables.</para>
        /// <para>Object to be serialized must have a parameterless constructor.</para>
        /// </summary>
        public static class JsonSerialization
        {
            /// <summary>
            /// Writes the given object instance to a Json file.
            /// <para>Object type must have a parameterless constructor.</para>
            /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
            /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
            /// </summary>
            /// <typeparam name="T">The type of object being written to the file.</typeparam>
            /// <param name="filePath">The file path to write the object instance to.</param>
            /// <param name="objectToWrite">The object instance to write to the file.</param>
            /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
            public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
            {
                TextWriter? writer = null;
                try
                {
                    var contentsToWriteToFile = Newtonsoft.Json.JsonConvert.SerializeObject(objectToWrite);
                    writer = new StreamWriter(filePath, append);
                    writer.Write(contentsToWriteToFile);
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }

            /// <summary>
            /// Reads an object instance from an Json file.
            /// <para>Object type must have a parameterless constructor.</para>
            /// </summary>
            /// <typeparam name="T">The type of object to read from the file.</typeparam>
            /// <param name="filePath">The file path to read the object instance from.</param>
            /// <returns>Returns a new instance of the object read from the Json file.</returns>
            public static T? ReadFromJsonFile<T>(string filePath, ref bool success) where T : new()
            {
                TextReader? reader = null;

                success = true;

                try
                {

                    reader = new StreamReader(filePath);
                    var fileContents = reader.ReadToEnd();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(fileContents)!;
                }
                catch (Exception e)
                {
                    success = false;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR in file '{0}' : {1} ", filePath, e.Message);
                    Console.ResetColor();


                    return default;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }

    }
}
