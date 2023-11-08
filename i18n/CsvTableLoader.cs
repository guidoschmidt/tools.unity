using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Localization;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace i18n
{

  /*
    Usage: 

    #if UNITY_EDITOR
    [Button("Assign CSV Table Provider")]
    public static void AssignTableProvider()
    {
    // Create an instance of the table provider.
    var provider = new CsvTableLoader();

    // A provider can be assigned to each database or the same provider can be shared between both.
    var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
    settings.GetStringDatabase().TableProvider = provider;
    // We do not have any translated assets (yet)
    //settings.GetAssetDatabase().TableProvider = provider;

    // Set dirty so the changes are saved.
    EditorUtility.SetDirty(settings);
    }
    #endif
  */
  [Serializable]
  public class CsvTableLoader : ITableProvider
  {
    public AsyncOperationHandle<TTable> ProvideTableAsync<TTable>(string tableName, Locale locale) where TTable : LocalizationTable
    {
      string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Localization", $"{tableName}.csv");
      Debug.Log($"[Localisation] Loading localisation CSV from {filePath}");

      if (typeof(TTable) == typeof(StringTable))
      {
        // Create the table and its shared table data.
        var table = ScriptableObject.CreateInstance<StringTable>();
        table.SharedData = ScriptableObject.CreateInstance<SharedTableData>();
        table.SharedData.TableCollectionName = tableName;
        table.LocaleIdentifier = locale.Identifier;

        List<Dictionary<string, object>> data = CsvReader.Read(filePath);
        Debug.Log(
          $"[Localisation] Requested {locale.LocaleName} {typeof(TTable).Name} with the name `{tableName}`.");


        var rowOneKeys = data[0].Keys;
        foreach (var key in rowOneKeys)
        {
          //Debug.Log($"Key {key.ToString()}");
        }

        var selectedLocaleIdentifier = LocalizationSettings.SelectedLocale.Identifier.ToString();
        foreach (var t in data)
        {
          var key = t["Key"].ToString();
          try
          {
            var value = t[selectedLocaleIdentifier];
            //Debug.Log($"{value}");
            table.AddEntry(key, $"{value}");
          }
          catch (Exception e)
          {
            Debug.LogWarning($"Missing Translation\n{e.Message}\n{key}");
          }
        }
        return Addressables.ResourceManager.CreateCompletedOperation(table as TTable, null);
      }
      // Fallback to default table loading.
      return default;
    }
  }
}
