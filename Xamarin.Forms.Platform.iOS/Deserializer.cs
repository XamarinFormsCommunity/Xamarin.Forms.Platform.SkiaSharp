using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms.Internals;

#if __MOBILE__
namespace Xamarin.Forms.Platform.iOS
#else

namespace Xamarin.Forms.Platform.MacOS
#endif
{
	internal class Deserializer : IDeserializer
	{
		const string PropertyStoreFile = "PropertyStore.forms";

		public Task<IDictionary<string, object>> DeserializePropertiesAsync()
		{
			// Deserialize property dictionary to local storage
			// Make sure to use Internal
			return Task.Run(() =>
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				using (var stream = store.OpenFile(PropertyStoreFile, System.IO.FileMode.OpenOrCreate))
				using (var reader = XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max))
				{
					if (stream.Length == 0)
						return null;

					try
					{
						var dcs = new DataContractSerializer(typeof(Dictionary<string, object>));
						return (IDictionary<string, object>)dcs.ReadObject(reader);
					}
					catch (Exception e)
					{
						Debug.WriteLine("Could not deserialize properties: " + e.Message);
						Log.Warning("Xamarin.Forms PropertyStore", $"Exception while reading Application properties: {e}");
					}
				}

				return null;
			});
		}

		public Task SerializePropertiesAsync(IDictionary<string, object> properties)
		{
			properties = new Dictionary<string, object>(properties);
			// Serialize property dictionary to local storage
			// Make sure to use Internal
			return Task.Run(() =>
			{
				var success = false;
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				using (var stream = store.OpenFile(PropertyStoreFile + ".tmp", System.IO.FileMode.OpenOrCreate))
				using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
				{
					try
					{
						var dcs = new DataContractSerializer(typeof(Dictionary<string, object>));
						dcs.WriteObject(writer, properties);
						writer.Flush();
						success = true;
					}
					catch (Exception e)
					{
						Debug.WriteLine("Could not serialize properties: " + e.Message);
						Log.Warning("Xamarin.Forms PropertyStore", $"Exception while writing Application properties: {e}");
					}
				}

				if (!success)
					return;
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					try
					{
						if (store.FileExists(PropertyStoreFile))
							store.DeleteFile(PropertyStoreFile);
						store.MoveFile(PropertyStoreFile + ".tmp", PropertyStoreFile);
					}
					catch (Exception e)
					{
						Debug.WriteLine("Could not move new serialized property file over old: " + e.Message);
						Log.Warning("Xamarin.Forms PropertyStore", $"Exception while writing Application properties: {e}");
					}
				}
			});
		}
	}
}