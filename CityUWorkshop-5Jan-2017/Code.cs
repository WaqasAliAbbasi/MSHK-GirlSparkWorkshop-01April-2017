////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 1                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
using System.Text;
using System;
namespace Core
{
    public static class PhonewordTranslator
    {
        public static string ToNumber(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";
            else
                raw = raw.ToUpperInvariant();

            var newNumber = new StringBuilder();
            newNumber.Append("+");
            foreach (var c in raw)
            {
                if (" -0123456789".Contains(c))
                    newNumber.Append(c);
                else
                {
                    var result = TranslateToNumber(c);
                    if (result != null)
                        newNumber.Append(result);
                }
                // otherwise we've skipped a non-numeric char
            }
            return newNumber.ToString();
        }
        static bool Contains(this string keyString, char c)
        {
            return keyString.IndexOf(c) >= 0;
        }
        static int? TranslateToNumber(char c)
        {
            if ("ABC".Contains(c))
                return 2;
            else if ("DEF".Contains(c))
                return 3;
            else if ("GHI".Contains(c))
                return 4;
            else if ("JKL".Contains(c))
                return 5;
            else if ("MNO".Contains(c))
                return 6;
            else if ("PQRS".Contains(c))
                return 7;
            else if ("TUV".Contains(c))
                return 8;
            else if ("WXYZ".Contains(c))
                return 9;
            return null;
        }
    }
}
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 2                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

// Get our UI controls from the loaded layout:
EditText phoneNumberText = FindViewById<EditText>(Resource.Id.PhoneNumberText);
Button translateButton = FindViewById<Button>(Resource.Id.TranslateButton);
Button callButton = FindViewById<Button>(Resource.Id.CallButton);

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 3                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

    // Disable the "Call" button
callButton.Enabled = false;

// Add code to translate number
string translatedNumber = string.Empty;

translateButton.Click += (object sender, EventArgs e) =>
{
    // Translate user's alphanumeric phone number to numeric
    translatedNumber = Core.PhonewordTranslator.ToNumber(phoneNumberText.Text);
    if (String.IsNullOrWhiteSpace(translatedNumber))
    {
        callButton.Text = "Call";
        callButton.Enabled = false;
    }
    else
    {
        callButton.Text = "Call " + translatedNumber;
        callButton.Enabled = true;
    }
};

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 4                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

callButton.Click += (object sender, EventArgs e) =>
{
    // On "Call" button click, try to dial phone number.
    var callDialog = new AlertDialog.Builder(this);
    callDialog.SetMessage("Call " + translatedNumber + "?");
    callDialog.SetNeutralButton("Call", delegate
    {
        // Create intent to dial phone
        var callIntent = new Intent(Intent.ActionCall);
        callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
        StartActivity(callIntent);
    });
    callDialog.SetNegativeButton("Cancel", delegate { });

    // Show the alert dialog to the user and wait for response.
    callDialog.Show();
};

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 5                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

<? xml version="1.0" encoding="utf-8"?>
<resources>
    <string name = "callHistory" > Call History</string>
</resources>

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 6                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Widget;
namespace Phoneword
{
    [Activity(Label = "@string/callHistory")]
    public class CallHistoryActivity : ListActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Create your application here
            var phoneNumbers = Intent.Extras.GetStringArrayList("phone_numbers") ?? new string[0];
            this.ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, phoneNumbers);
        }
    }
}

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 7                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

//Should look like this, dont copy the second line (it's already there)
[Activity(Label = "Phoneword", MainLauncher = true, Icon = "@drawable/icon")]
public class MainActivity : Activity
{
    static readonly List<string> phoneNumbers = new List<string>();
    ...// OnCreate, etc.
}

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 8                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

Button callHistoryButton = FindViewById<Button>(Resource.Id.CallHistoryButton);
callHistoryButton.Click += (sender, e) =>
{
    var intent = new Intent(this, typeof(CallHistoryActivity));
intent.PutStringArrayListExtra("phone_numbers", phoneNumbers);
    StartActivity(intent);
};

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 9                        ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

callDialog.SetNeutralButton("Call", delegate
{
    // add dialed number to list of called numbers.
    phoneNumbers.Add(translatedNumber);
    // enable the Call History button
    callHistoryButton.Enabled = true;
    // Create intent to dial phone
    var callIntent = new Intent(Intent.ActionCall);
callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
    StartActivity(callIntent);
});

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 10                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

EditText sex = FindViewById<EditText>(Resource.Id.sex);
EditText age = FindViewById<EditText>(Resource.Id.age);
EditText education = FindViewById<EditText>(Resource.Id.education);
EditText maritalStatus = FindViewById<EditText>(Resource.Id.maritalstatus);
EditText race = FindViewById<EditText>(Resource.Id.race);
EditText nativecountry = FindViewById<EditText>(Resource.Id.nativecountry);
Button predictIncomeButton = FindViewById<Button>(Resource.Id.PredictIncomeButton);

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 11                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

predictIncomeButton.Click += (Object Sender, EventArgs e) =>
{
    List<string> colNames = new List<string> { "age", "education", "marital-status", "race", "sex", "native-country" };
    List<string> colVals = new List<string>
    {
        age.Text,
        education.Text,
        maritalStatus.Text,
        race.Text,
        sex.Text,
        nativecountry.Text
    };
    
    InvokeRequestResponseService(colNames, colVals);
};

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 12                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

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

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 13                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

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
        const string apiKey = "API KEY"; // Replace this with the API key for the web service
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        client.BaseAddress = new Uri("API URL"); //Replace this with the API Url for the web service

        var json = JsonConvert.SerializeObject(scoreRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Result: {0}", result);
        }
        else
        {
            Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

            // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
            Console.WriteLine(response.Headers.ToString());

            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
        }
    }
}

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 14                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

public class StringTable
{
    public string[] ColumnNames { get; set; }
    public string[,] Values { get; set; }
}

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 15                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

//Show it to user
var Dialog = new AlertDialog.Builder(this);
Dialog.SetMessage(result);
Dialog.SetNeutralButton("Ok", delegate { });

// Show the Result dialog and wait for response
Dialog.Show();

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 16                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

string result = string.Format("The request failed with status code: {0}", response.StatusCode);

//Show it to user
var Dialog = new AlertDialog.Builder(this);
Dialog.SetMessage(result);
Dialog.SetNeutralButton("Ok", delegate { });

// Show the Result dialog and wait for response
Dialog.Show();

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 17                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

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

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 18                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

<? xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:orientation="vertical"
    android:layout_width="match_parent"
    android:layout_height="match_parent">
    <TextView
        android:text="Gender"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView1" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/sex" />
    <TextView
        android:text="Age"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView2" />
    <EditText
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/age"
        android:hint="24"
        android:digits="0123456789" />
    <TextView
        android:text="Education"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView3" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/education" />
    <TextView
        android:text="Marital Status"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView4" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/maritalstatus" />
    <TextView
        android:text="Native Country"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView5" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/nativecountry" />
    <TextView
        android:text="Race"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView6" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/race" />
    <Button
        android:text="Predict"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/PredictIncomeButton" />
</LinearLayout>

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 19                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

<?xml version="1.0" encoding="utf-8"?>
<resources>
  <string name="ApplicationName">USA Income Prediction</string>
  <string-array name="education_array">
    <item>Preschool</item>
    <item>1st-4th</item>
    <item>5th-6th</item>
    <item>7th-8th</item>
    <item>9th</item>
    <item>10th</item>
    <item>11th</item>
    <item>12th</item>
    <item>HS-grad</item>
    <item>Some-college</item>
    <item>Bachelors</item>
    <item>Masters</item>
    <item>Doctorate</item>
    <item>Assoc-acdm</item>
    <item>Assoc-voc</item>
    <item>Prof-school</item>
  </string-array>

  <string-array name="maritalstatus_array">
    <item>Never-married</item>
    <item>Married-civ-spouse</item>
    <item>Married-AF-spouse</item>
    <item>Married-spouse-absent</item>
    <item>Separated</item>
    <item>Divorced</item>
    <item>Widowed</item>
  </string-array>

  <string-array name="nativecountry_array">
    <item>Cambodia</item>
    <item>Canada</item>
    <item>China</item>
    <item>Columbia</item>
    <item>Cuba</item>
    <item>Dominican-Republic</item>
    <item>Ecuador</item>
    <item>El-Salvador</item>
    <item>England</item>
    <item>France</item>
    <item>Germany</item>
    <item>Greece</item>
    <item>Guatemala</item>
    <item>Haiti</item>
    <item>Holand-Netherlands</item>
    <item>Honduras</item>
    <item>Hong</item>
    <item>Hungary</item>
    <item>India</item>
    <item>Iran</item>
    <item>Ireland</item>
    <item>Italy</item>
    <item>Jamaica</item>
    <item>Japan</item>
    <item>Laos</item>
    <item>Mexico</item>
    <item>Nicaragua</item>
    <item>Outlying-US(Guam-USVI-etc)</item>
    <item>Peru</item>
    <item>Philippines</item>
    <item>Poland</item>
    <item>Portugal</item>
    <item>Puerto-Rico</item>
    <item>Scotland</item>
    <item>South</item>
    <item>Taiwan</item>
    <item>Thailand</item>
    <item>Trinadad&amp;Tobago</item>
    <item>United-States</item>
    <item>Vietnam</item>
    <item>Yugoslavia</item>
  </string-array>

  <string-array name="race_array">
    <item>Amer-Indian-Eskimo</item>
    <item>Asian-Pac-Islander</item>
    <item>Black</item>
    <item>White</item>
    <item>Other</item>
  </string-array>

  <string-array name="sex_array">
    <item>Male</item>
    <item>Female</item>
  </string-array>
</resources>

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 20                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

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

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 21                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

predictIncomeButton.Click += (Object Sender, EventArgs e) =>
{
    List<string> colNames = new List<string> { "age", "education", "marital-status", "race", "sex", "native-country" };
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

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 22                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

<? xml version="1.0" encoding="utf-8"?>
<LinearLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:orientation="vertical"
    android:layout_width="match_parent"
    android:layout_height="match_parent">
    <TextView
        android:text="Gender"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView1" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/sex"
        android:spinnerMode="dialog"
        android:prompt="@+string/sexprompt" />
    <TextView
        android:text="Age"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView2" />
    <EditText
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/age"
        android:hint="24"
        android:digits="0123456789" />
    <TextView
        android:text="Education"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView3" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/education"
        android:spinnerMode="dialog"
        android:prompt="@+string/educationprompt" />
    <TextView
        android:text="Marital Status"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView4" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/maritalstatus"
        android:spinnerMode="dialog"
        android:prompt="@+string/maritalstatusprompt" />
    <TextView
        android:text="Native Country"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView5" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/nativecountry"
        android:spinnerMode="dialog"
        android:prompt="@+string/nativecountryprompt" />
    <TextView
        android:text="Race"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/textView6" />
    <Spinner
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/race"
        android:spinnerMode="dialog"
        android:prompt="@+string/raceprompt" />
    <Button
        android:text="Predict"
        android:layout_width="match_parent"
        android:layout_height="wrap_content"
        android:id="@+id/PredictIncomeButton" />
</LinearLayout>

////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////
//////                     Code 23                       ///////
////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////

<string name = "sexprompt" > Select Gender</string>
<string name = "educationprompt" > Select Education</string>
<string name = "maritalstatusprompt" > Select Marital Status</string>
<string name = "nativecountryprompt" > Select Country</string>
<string name = "raceprompt" > Select Race</string>