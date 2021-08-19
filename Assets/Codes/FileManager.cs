using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
#if UNITY_EDITOR 
using UnityEditor;
#endif 

namespace EES.Utilities
{
    public class FileManager
    {
        /// <summary>
        /// Where did we save our data?
        /// </summary>
        static public string RootDirectory
        {
            get
            {
                string dir = Application.persistentDataPath + "/Documents";
                if (!Directory.Exists(dir))
                {
                    Debug.Log("Creating path: " + dir);
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }
        /// <summary>
        /// Automatically serialize object with Json.net and save it
        /// </summary>
        /// <returns><c>true</c>, if file was saved, <c>false</c> otherwise.</returns>
        /// <param name="_fileName">File name.</param>
        /// <param name="obJectToSave">Save item.</param>
        public static bool SaveFile(string _fileName, object obJectToSave)
        {
            if (obJectToSave == null)//Nothing to save
                return false;

            string _serializedData = JsonConvert.SerializeObject(obJectToSave);
            return SaveFile(_fileName, _serializedData);
        }

        /// <summary>
        /// Load string from files and automatically convert to object with Json.net
        /// </summary>
        /// <returns>Deserialized object</returns>
        /// <param name="_fileName">File name or path.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        public static T LoadFile<T>(string _fileName) where T : class
        {
            string _data = LoadFile(_fileName);
            if (_data == null)
                return null;

            T _targetObject = JsonConvert.DeserializeObject<T>(_data);
            return _targetObject;
        }

        /// <summary>
        /// Save file to disk
        /// </summary>
        /// <param name="_fileName">File name gonna to save</param>
        /// <param name="_data">Strings to be written to files</param>
        /// <returns></returns>
        public static bool SaveFile(string _fileName, string _data)
        {
            string _filePath = RootDirectory + "/" + _fileName;
            if (!File.Exists(_filePath))
            {
                Debug.Log("Creating file: " + _filePath);
                FileStream _fs = File.Create(_filePath);
                _fs.Dispose();
            }

            File.WriteAllText(
                    _filePath,
                    _data
                );
            return true;
        }

        /// <summary>
        /// Load files using file name
        /// </summary>
        /// <param name="_fileName">file name to load</param>
        /// <returns>Data read from file. Will be <see cref="null"/> if file didn't exist</returns>
        public static string LoadFile(string _fileName)
        {
            string _filePath = RootDirectory + "/" + _fileName;
            if (!File.Exists(_filePath))
            {
                return null;
            }
            else
            {
                return File.ReadAllText(_filePath);
            }
        }

        public static byte[] LoadImageFile(string _fileName)
        {
            string _filePath = RootDirectory + "/" + _fileName;
            if (!File.Exists(_filePath))
            {
                return null;
            }
            else
            {
                return File.ReadAllBytes(_filePath);
            }
        }

        public static bool SaveImageFile(string _fileName, Texture2D _tex)
        {
            string _filePath = RootDirectory + "/" + _fileName;
            if (!File.Exists(_filePath))
            {
                Debug.Log("Creating file: " + _filePath);
                FileStream _fs = File.Create(_filePath);
                _fs.Dispose();
            }
            if (_tex == null)
                return false;

            File.WriteAllBytes(_filePath, _tex.EncodeToPNG());

            return true;
        }

        public static bool Delete(string _fileName)
        {
            string _filePath = RootDirectory + "/" + _fileName;
            if (File.Exists(_filePath))
                File.Delete(_filePath);
            else
                Debug.Log(_filePath + "is not found.");
            return true;
        }

        #if UNITY_EDITOR 
        [MenuItem("EESTools/Clear App Persistent Data")]
        static void deleteAllFiles ()
        {
            if (EditorUtility.DisplayDialog("Delete App Persistent Data.",
                    "This will delete all files under Application.persistentDataPath." +
                    "\nAre you sure?" +
                    "\nThis action cannot be undo.", "Yes", "No"))
            {
                Debug.Log("Removing files under: " + Application.persistentDataPath);
                FileUtil.DeleteFileOrDirectory(Application.persistentDataPath);
            }
        }
        #endif
    }
}