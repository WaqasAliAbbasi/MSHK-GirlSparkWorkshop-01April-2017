using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Widget;
using Android.OS;

using Newtonsoft.Json;

namespace IncomePrediction
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }

    [Activity(Label = "USA Income Class Prediction", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //EditText sex = FindViewById<EditText>(Resource.Id.sex);
            EditText age = FindViewById<EditText>(Resource.Id.age);
            //EditText education = FindViewById<EditText>(Resource.Id.education);
            //EditText maritalStatus = FindViewById<EditText>(Resource.Id.maritalstatus);
            //EditText race = FindViewById<EditText>(Resource.Id.race);
            //EditText nativecountry = FindViewById<EditText>(Resource.Id.nativecountry);
            Button predictIncomeButton = FindViewById<Button>(Resource.Id.PredictIncomeButton);

            Spinner education = FindViewById<Spinner>(Resource.Id.education);
            var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.education_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            education.Adapter = adapter;


            Spinner maritalStatus = FindViewById<Spinner>(Resource.Id.maritalstatus);
            var adapter2 = ArrayAdapter.CreateFromResource(this, Resource.Array.maritalstatus_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            maritalStatus.Adapter = adapter2;


            Spinner race = FindViewById<Spinner>(Resource.Id.race);
            var adapter3 = ArrayAdapter.CreateFromResource(this, Resource.Array.race_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter3.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            race.Adapter = adapter3;

            Spinner sex = FindViewById<Spinner>(Resource.Id.sex);
            var adapter4 = ArrayAdapter.CreateFromResource(this, Resource.Array.sex_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter4.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            sex.Adapter = adapter4;

            Spinner nativecountry = FindViewById<Spinner>(Resource.Id.nativecountry);
            var adapter5 = ArrayAdapter.CreateFromResource(this, Resource.Array.nativecountry_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter5.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            nativecountry.Adapter = adapter5;

            predictIncomeButton.Click += (Object Sender, EventArgs e) =>
            {
                List<string> colNames = new List<string> { "age", "education", "marital-status", "race", "sex","native-country" };
                List<string> colVals = new List<string> {
                    age.Text,
                    education.GetItemAtPosition(education.SelectedItemPosition).ToString(),
                    maritalStatus.GetItemAtPosition(maritalStatus.SelectedItemPosition).ToString(),
                    race.GetItemAtPosition(race.SelectedItemPosition).ToString(),
                    sex.GetItemAtPosition(sex.SelectedItemPosition).ToString(),
                    nativecountry.GetItemAtPosition(nativecountry.SelectedItemPosition).ToString()
                };

                InvokeRequestResponseService(colNames, colVals);
            };

            async void InvokeRequestResponseService(List<string> colNames, List<string> colValues)
            {
                //Column names and values
                StringTable stringTable = new StringTable();
                stringTable.ColumnNames = colNames.ToArray();

                int i = 0;
                int rowCnt = 1;//only a single row of input
                stringTable.Values = new string[rowCnt, colValues.Count];
                foreach (string item in colValues)
                {
                    stringTable.Values[0, i] = item;
                    i++;
                }

                using (var client = new HttpClient())
                {
                    var scoreRequest = new
                    {

                        Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            stringTable
                        },
                    },
                        GlobalParameters = new Dictionary<string, string>()
                        {
                        }
                    };
                    const string apiKey = "nH1fQBtXcWOSLrddNYJG4Bqx7l70xzXUP0p8t0s/M61oa5TCEhLmhskE0K4eURcNIZ8VfJugethCOVMyJ6kscg=="; // Replace this with the API key for the web service
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                    client.BaseAddress = new Uri("https://asiasoutheast.services.azureml.net/workspaces/50edb6832d9e4932bc7493e8fbc9e5c5/services/3b8561f2564344c588973c17228456d6/execute?api-version=2.0&details=true");

                    // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                    // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                    // For instance, replace code such as:
                    //      result = await DoSomeTask()
                    // with the following:
                    //      result = await DoSomeTask().ConfigureAwait(false)

                    var json = JsonConvert.SerializeObject(scoreRequest);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        
                        //Refine the result
                        if (result.Contains("Low"))
                        {
                            result = "Predicted Income Class is Low.";
                        }
                        else if (result.Contains("Medium"))
                        {
                            result = "Predicted Income Class is Medium";
                        }
                        else if(result.Contains("High"))
                        {
                            result = "Predicted Income Class is High.";
                        }
                        else if(result.Contains("Very High"))
                        {
                            result = "Predicted Income Class is Very High.";
                        }

                        //Show it to user
                        var Dialog = new AlertDialog.Builder(this);
                        Dialog.SetMessage(result);
                        Dialog.SetNeutralButton("Ok", delegate { });

                        // Show the Result dialog and wait for response
                        Dialog.Show();
                    }
                    else
                    {
                        string result = string.Format("The request failed with status code: {0}", response.StatusCode);

                        //Show it to user
                        var Dialog = new AlertDialog.Builder(this);
                        Dialog.SetMessage(result);
                        Dialog.SetNeutralButton("Ok", delegate { });

                        // Show the Result dialog and wait for response
                        Dialog.Show();
                    }
                }
            }
        }
    }   
}

