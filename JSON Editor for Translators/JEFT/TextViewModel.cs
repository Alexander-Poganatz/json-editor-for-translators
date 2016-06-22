using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON_Editor_for_Translators.JEFT
{
    public class TextViewModel
    {
        public ObservableCollection<TextModel> TextList;

        private String IsExportedFileKey = "IsExportedFile";
        private String ArrayKey = "Data";
        private String KeyKey = "Key";
        private String ValueKey = "Value";
        private String NewValueKey = "NewValue";

        private bool LastFileHandledIsExported;

        public bool HandledExportedLast { get { return LastFileHandledIsExported; } }

        public TextViewModel()
        {
            TextList = new ObservableCollection<TextModel>();
            LastFileHandledIsExported = false;
            //Test data for now:
            TextList.Add(new TextModel { Key = "Title", Value = "Json Editor for Translators", NewValue = "Json Editor for Translators" });
            TextList.Add(new TextModel { Key = "Des", Value = "An app to help translators edit certain json files.", NewValue = "Une application pour aider les traducteurs modifier certains fichiers JSON" });
            TextList.Add(new TextModel { Key = "Other", Value = "I only know English :(", NewValue = "Je connais seulement l'anglais" });
        }

        public bool LoadJson(String jsonString)
        {
            Boolean success = true;
            Windows.Data.Json.JsonObject jsonObject = Windows.Data.Json.JsonObject.Parse(jsonString);

            Boolean isExported = jsonObject.GetNamedBoolean(IsExportedFileKey);


            if(isExported)
            {
                LoadExportedJson(jsonString);
            }
            else
            {
                LastFileHandledIsExported = false;

                Windows.Data.Json.JsonArray jsonArray = jsonObject.GetNamedArray(ArrayKey);
                Windows.Data.Json.JsonObject tempObject;

                //Erase current list.
                TextList.Clear();
                for(uint x = 0; x < jsonArray.Count; ++x)
                {
                    tempObject = jsonArray.GetObjectAt(x);

                    TextList.Add(new TextModel
                    {
                        Key = tempObject.GetNamedString(KeyKey),
                        Value = tempObject.GetNamedString(ValueKey),
                        NewValue = tempObject.GetNamedString(NewValueKey)
                    });
                }
            }

            return success;
        }
        public void LoadExportedJson(String jsonString)
        {
            Windows.Data.Json.JsonObject jsonObject = Windows.Data.Json.JsonObject.Parse(jsonString);

            var jsonObjectKeys = jsonObject.Keys;

            TextList.Clear();
            for(int x = 0; x < jsonObjectKeys.Count; ++x)
            {
                if (jsonObjectKeys.ElementAt(x) != IsExportedFileKey)
                    TextList.Add(new TextModel
                    {
                        Key = jsonObjectKeys.ElementAt(x),
                        Value = "",
                        NewValue = jsonObject.GetNamedString(jsonObjectKeys.ElementAt(x))
                    });
            }
            LastFileHandledIsExported = true;
        }

        public String toJsonString()
        {


            Windows.Data.Json.JsonObject MyJsonObject = new Windows.Data.Json.JsonObject();
            MyJsonObject.Add(IsExportedFileKey, Windows.Data.Json.JsonValue.CreateBooleanValue(false));

            Windows.Data.Json.JsonArray MyJsonArray = new Windows.Data.Json.JsonArray();

            Windows.Data.Json.JsonObject tempObject;

            for(int x = 0; x < TextList.Count; ++x)
            {
                tempObject = new Windows.Data.Json.JsonObject();
                tempObject.Add(KeyKey, Windows.Data.Json.JsonValue.CreateStringValue(TextList.ElementAt(x).Key));
                tempObject.Add(ValueKey, Windows.Data.Json.JsonValue.CreateStringValue(TextList.ElementAt(x).Value));
                tempObject.Add(NewValueKey, Windows.Data.Json.JsonValue.CreateStringValue(TextList.ElementAt(x).NewValue));

                MyJsonArray.Add(tempObject);
            }

            MyJsonObject.Add(ArrayKey, MyJsonArray);

            //Probably means I handled non exported file last.
            LastFileHandledIsExported = false;

            return MyJsonObject.Stringify();

        }

        public string toExportJsonString()
        {
            //Copy pasting from above:
            Windows.Data.Json.JsonObject MyJsonObject = new Windows.Data.Json.JsonObject();
            MyJsonObject.Add(IsExportedFileKey, Windows.Data.Json.JsonValue.CreateBooleanValue(true));

            //Add all the 
            for (int x = 0; x < TextList.Count; ++x)
            {
                MyJsonObject.Add(TextList.ElementAt(x).Key,
                    Windows.Data.Json.JsonValue.CreateStringValue(TextList.ElementAt(x).NewValue));
            }

            //return MyJsonObject.Stringify();
            String TempString = MyJsonObject.Stringify();
            String ExportString = "";

            for (int x = 0; x < TempString.Length; ++x)
            {
                if (TempString.ElementAt(x) == ',')
                {
                    ExportString += ',';
                    ExportString += '\n';
                }
                else
                    ExportString += TempString.ElementAt(x);
            }

            //Probably means I handled exported file last.
            LastFileHandledIsExported = true;

            return ExportString;

        }

        public void AddItem()
        {
            TextList.Add(new TextModel { Key = "", Value = "", NewValue = "" });
        }
    }
}
