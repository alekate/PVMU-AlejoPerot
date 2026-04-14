using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;

public class WeatherRequester : MonoBehaviour
{
    [Header("Weather Information")]
    [SerializeField] private TMP_Text cityNameText;
    [SerializeField] private TMP_Text temperatureText;
    [SerializeField] private TMP_Text humidityText;
    [SerializeField] private TMP_Text pressureText;

    [Header("Search Field")]
    [SerializeField] private Button searchButton;
    [SerializeField] private TMP_InputField cityInput;

    [Header("API Key")]
    [SerializeField] private string apiKey;
    //57c75b2e0fc0a6107e9a324ab8b26550

    private void Start()
    {
        searchButton.onClick.AddListener(RequestWeatherForecast);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RequestWeatherForecast();
        }
    }

    private IEnumerator SendGetRequest<T> (string requestUrl, Action<T> onSuccess, Action<string> onError)
    {
        T response;

        UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            response = JsonUtility.FromJson<T>(webRequest.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else
        {
            onError?.Invoke(webRequest.error); 
        }

        webRequest.Dispose();
    }

    private float KelvinToCelsius(float k) => k - 273.15f; //Funcion de pasaje Kelvin a Celcius

    public void RequestWeatherForecast()
    {
        string city = UnityWebRequest.EscapeURL(cityInput.text);
        string requestUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

        StartCoroutine(SendGetRequest<WeatherData>(requestUrl,
            onSuccess: (data) =>
            {
                cityNameText.text = cityInput.text.ToUpper();
                temperatureText.text = KelvinToCelsius(data.main.temp).ToString("F1") + "°C";
                humidityText.text = data.main.humidity + "%";
                pressureText.text = data.main.pressure + "hPa";
            },
            onError: (error) =>
            {
                Debug.LogError(error);
            }));
    }
}
