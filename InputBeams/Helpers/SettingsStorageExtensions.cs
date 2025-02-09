using InputBeams.Core.Helpers;

using Windows.Storage;
using Windows.Storage.Streams;

namespace InputBeams.Helpers;

// Use these extension methods to store and retrieve local and roaming app data
// More details regarding storing and retrieving app data at https://docs.microsoft.com/windows/apps/design/app-settings/store-and-retrieve-app-data
public static class SettingsStorageExtensions
{
    private const string FileExtension = ".json";

    public static bool IsRoamingStorageAvailable(this ApplicationData appData)
    {
        return appData.RoamingStorageQuota == 0;
    }

    public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
    {
        var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.ReplaceExisting);
        var fileContent = await Json.StringifyAsync(content);

        await FileIO.WriteTextAsync(file, fileContent);
    }

    public static async Task<T?> ReadAsync<T>(this StorageFolder folder, string name)
    {
        if (!File.Exists(Path.Combine(folder.Path, GetFileName(name))))
        {
            return default;
        }

        var file = await folder.GetFileAsync($"{name}.json");
        var fileContent = await FileIO.ReadTextAsync(file);

        return await Json.ToObjectAsync<T>(fileContent);
    }

    public static async Task SaveAsync<T>(this ApplicationDataContainer settings, string key, T value)
    {
        settings.SaveString(key, await Json.StringifyAsync(value));
    }

    public static void SaveString(this ApplicationDataContainer settings, string key, string value)
    {
        settings.Values[key] = value;
    }

    public static async Task<T?> ReadAsync<T>(this ApplicationDataContainer settings, string key)
    {
        object? obj;

        if (settings.Values.TryGetValue(key, out obj))
        {
            return await Json.ToObjectAsync<T>((string)obj);
        }

        return default;
    }

    public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name is null or empty. Specify a valid file name", nameof(fileName));
        }

        var storageFile = await folder.CreateFileAsync(fileName, options);
        await FileIO.WriteBytesAsync(storageFile, content);
        return storageFile;
    }

    public static async Task<byte[]?> ReadFileAsync(this StorageFolder folder, string fileName)
    {
        var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

        if ((item != null) && item.IsOfType(StorageItemTypes.File))
        {
            var storageFile = await folder.GetFileAsync(fileName);
            var content = await storageFile.ReadBytesAsync();
            return content;
        }

        return null;
    }

    public static async Task<byte[]?> ReadBytesAsync(this StorageFile file)
    {
        if (file != null)
        {
            using IRandomAccessStream stream = await file.OpenReadAsync();
            using var reader = new DataReader(stream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)stream.Size);
            var bytes = new byte[stream.Size];
            reader.ReadBytes(bytes);
            return bytes;
        }

        return null;
    }

    private static string GetFileName(string name)
    {
        return string.Concat(name, FileExtension);
    }

    public static void SaveSetting<T>(this ApplicationDataContainer settings, string key, T value)
    {
        if (value is bool boolValue)
        {
            settings.Values[key] = boolValue; // ✅ Store real boolean
        }
        else
        {
            settings.Values[key] = value; // ✅ Store other types normally
        }
    }

    public static T LoadSetting<T>(this ApplicationDataContainer settings, string key, T defaultValue = default)
    {
        if (settings.Values.TryGetValue(key, out object obj))
        {
            try
            {
                if (obj is T value)
                {
                    return value; // ✅ Return directly if type matches
                }

                Type targetType = typeof(T);

                if (targetType == typeof(bool))
                {
                    if (obj is string strValue)
                    {
                        if (bool.TryParse(strValue, out bool boolResult))
                        {
                            return (T)(object)boolResult; // ✅ Convert stored string to bool
                        }
                    }
                    else if (obj is int intValue)
                    {
                        return (T)(object)(intValue != 0); // ✅ Convert 1/0 to true/false
                    }
                }
                else if (targetType == typeof(int) && obj is string intStr)
                {
                    if (int.TryParse(intStr, out int intResult))
                    {
                        return (T)(object)intResult; // ✅ Convert stored string to int
                    }
                }

                System.Diagnostics.Debug.WriteLine($"⚠️ Type mismatch for key '{key}': Expected {targetType}, found {obj.GetType()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading setting '{key}': {ex.Message}");
            }
        }

        return defaultValue; // ✅ Return default if value not found or invalid
    }


}
