using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;


using UnityEngine;
using System.Collections;

public class FileLoader<T>{
	public void WriteToFile(T obj, string filePath)
	{
		using (FileStream fs = new FileStream(filePath, FileMode.Create))
		{
			using (BinaryWriter w = new BinaryWriter(fs))
			{
				w.Write(SerializeObject(obj));
			}
		}
	}
	
	public T LoadFromFile(string filePath)
	{
		if(!File.Exists(filePath))
		{
			Debug.LogWarning("File not found! Check your spelling");
			return default(T);
		}
		using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader r = new BinaryReader(fs))
			{
				Byte[] deserialiseThis = new Byte[fs.Length];
				int remaining = (int)fs.Length;
				int offset = 0;
				//Debug.Log(remaining);
				while(remaining > 0)
				{
					int read = r.Read(deserialiseThis, offset, remaining);
					if(read <= 0)
					{
						Debug.LogError("End of filestream reached with some bytes left to read!");
					}
					remaining -= read;
					offset += read;
				}
				if(deserialiseThis.Length > 0)
				{
					T retV = DeserializeObject(deserialiseThis);
					return retV;
				} else {
					return default(T);
				}
			}
		}
		//return retV;
	}
	
	byte[] SerializeObject(T objectToSerialize)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream memStr = new MemoryStream();

        bf.Serialize(memStr, objectToSerialize);
		memStr.Position = 0;
        
		//return "";
       return memStr.ToArray();
    }
	
	T DeserializeObject(byte[] dataStream)
	{
		MemoryStream stream = new MemoryStream(dataStream);
		stream.Position = 0;
		BinaryFormatter bf = new BinaryFormatter();
		bf.Binder = new VersionFixer();
		T retV = (T)bf.Deserialize(stream);
		return retV;
	}

}

sealed class VersionFixer : SerializationBinder 
{
    public override Type BindToType(string assemblyName, string typeName) 
    {
        Type typeToDeserialize = null;

        // For each assemblyName/typeName that you want to deserialize to
        // a different type, set typeToDeserialize to the desired type.
        String assemVer1 = Assembly.GetExecutingAssembly().FullName;

        if (assemblyName != assemVer1) 
        {
            // To use a type from a different assembly version, 
            // change the version number.
            // To do this, uncomment the following line of code.
             assemblyName = assemVer1;

            // To use a different type from the same assembly, 
            // change the type name.
        }

        // The following line of code returns the type.
        typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

        return typeToDeserialize;
	}
	
}