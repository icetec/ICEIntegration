// ##############################################################################
//
// ICEWorldEnvironmentAdapter.cs
// Version 1.2
//
// © Pit Vetterick, ICE Technologies Consulting LTD. All Rights Reserved.
// http://www.icecreaturecontrol.com
// mailto:support@icecreaturecontrol.com
// 
// Unity Asset Store End User License Agreement (EULA)
// http://unity3d.com/legal/as_terms
//
// ##############################################################################

using UnityEngine;
using System;
using System.Collections;

using ICE;
using ICE.World;
using ICE.World.EnumTypes;
using ICE.World.Utilities;
using ICE.World.Objects;

#if ICE_TENKOKU
using Tenkoku.Core;
#elif ICE_WEATHER_MAKER
using DigitalRuby.WeatherMaker;
#endif

namespace ICE.Integration.Adapter
{
#if ICE_ENVIRO
	public class ICEWorldEnvironmentAdapter : ICEWorldEnvironment 
	{
		private EnviroSky m_WeatherSystem = null;
		private EnviroSky WeatherSystem{
		get{
			if( m_WeatherSystem == null )
					m_WeatherSystem = (EnviroSky) FindObjectOfType(typeof(EnviroSky));

			return m_WeatherSystem;
			}
		}

		private float m_UpdateTimer = 0;
		public float UpdateInterval = 10;

		public override void Awake (){
			UpdateWeather();
		}


		public override void Update ()
		{
			m_UpdateTimer += Time.deltaTime;
			if( m_UpdateTimer < UpdateInterval )
				return;

			m_UpdateTimer = 0;

			UpdateWeather();

		}

		public void UpdateWeather()
		{
			if( WeatherSystem == null )
				Debug.LogWarning ("Sorry, WeatherMakerScript not exists!");

			WeatherForecast = ConvertWeatherType();
			//Temperature = WeatherSystem.temperature;

			DateDay = WeatherSystem.GameTime.Days;
			//DateMonth = WeatherSystem.GameTime.
			DateYear = WeatherSystem.GameTime.Years;
			TimeHour = WeatherSystem.GameTime.Hours;
			TimeMinutes = WeatherSystem.GameTime.Minutes;
			TimeSeconds = WeatherSystem.GameTime.Seconds;

			if( WeatherSystem.Weather.windZone != null )
			{
				WindSpeed = WeatherSystem.Weather.windZone.windMain;
				WindDirection = Quaternion.Angle( Quaternion.identity, WeatherSystem.Weather.windZone.transform.rotation );
			}

			/*
			UpdateTemperatureScale( TemperatureScaleType.FAHRENHEIT );

			Temperature = WeatherSystem.Temperature;
			MinTemperature = -100;
			MaxTemperature = 150;
			*/
		}

		private WeatherType ConvertWeatherType()
		{
			WeatherType _weather_type = WeatherType.UNDEFINED;
				/*
				if( WeatherSystem.Sky. != WeatherMakerPrecipitationType.None && WeatherSystem.PrecipitationIntensity > 0.75 )
			_weather_type = WeatherType.HEAVY_RAIN;
			else if( WeatherSystem.Precipitation != WeatherMakerPrecipitationType.None && WeatherSystem.PrecipitationIntensity > 0.25 )
			_weather_type = WeatherType.RAIN;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Heavy )
			_weather_type = WeatherType.MOSTLY_CLOUDY;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Medium )
			_weather_type = WeatherType.CLOUDY;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Light )
			_weather_type = WeatherType.PARTLY_CLOUDY;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Storm )
			_weather_type = WeatherType.STORMY;
			else if( WeatherSystem.WindScript != null && WeatherSystem.WindScript.WindIntensity > 0.25f )
			_weather_type = WeatherType.WINDY;
			else if( WeatherSystem.FogScript != null && WeatherSystem.FogScript.FogDensity > 0.25f  )
			_weather_type = WeatherType.FOGGY;
			else*/
			_weather_type = WeatherType.CLEAR;

			return _weather_type;
		}
	}
#elif ICE_WEATHER_MAKER
	public class ICEWorldEnvironmentAdapter : ICEWorldEnvironment 
	{
		private WeatherMakerScript m_WeatherSystem = null;
		private WeatherMakerScript WeatherSystem{
			get{
				if( m_WeatherSystem == null )
					m_WeatherSystem = (WeatherMakerScript) FindObjectOfType(typeof(WeatherMakerScript));

				return m_WeatherSystem;
			}
		}

		private float m_UpdateTimer = 0;
		public float UpdateInterval = 10;

		public override void Awake (){
			UpdateWeather();
		}


		public override void Update ()
		{

			m_UpdateTimer += Time.deltaTime;
			if( m_UpdateTimer < UpdateInterval )
			return;

			m_UpdateTimer = 0;

			UpdateWeather();


		}

		public void UpdateWeather()
		{
			if( WeatherSystem == null )
				Debug.LogWarning ("Sorry, WeatherMakerScript not exists!");

			WeatherForecast = ConvertWeatherType();
			//Temperature = WeatherSystem.temperature;

			TimeSpan _time = TimeSpan.FromSeconds(WeatherSystem.TimeOfDay);
			DateTime _date = new DateTime( _time.Ticks );

			DateDay = _date.Day;
			DateMonth = _date.Month;
			DateYear = _date.Year;
			TimeHour = _time.Hours;
			TimeMinutes = _time.Minutes;
			TimeSeconds = _time.Seconds;

			if( WeatherSystem.WindScript != null )
			{
				WindSpeed = WeatherSystem.WindScript.WindIntensity;
				WindDirection = Quaternion.Angle( Quaternion.identity, Quaternion.Euler( WeatherSystem.WindScript.WindDirection ) );
			}


			UpdateTemperatureScale( TemperatureScaleType.FAHRENHEIT );

			Temperature = WeatherSystem.Temperature;
			MinTemperature = -100;
			MaxTemperature = 150;
		}

		private WeatherType ConvertWeatherType()
		{
			WeatherType _weather_type = WeatherType.UNDEFINED;

			if( WeatherSystem.Precipitation != WeatherMakerPrecipitationType.None && WeatherSystem.PrecipitationIntensity > 0.75 )
				_weather_type = WeatherType.HEAVY_RAIN;
			else if( WeatherSystem.Precipitation != WeatherMakerPrecipitationType.None && WeatherSystem.PrecipitationIntensity > 0.25 )
				_weather_type = WeatherType.RAIN;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Heavy )
				_weather_type = WeatherType.MOSTLY_CLOUDY;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Medium )
				_weather_type = WeatherType.CLOUDY;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Light )
				_weather_type = WeatherType.PARTLY_CLOUDY;
			else if( WeatherSystem.Clouds != WeatherMakerCloudType.Storm )
				_weather_type = WeatherType.STORMY;
			else if( WeatherSystem.WindScript != null && WeatherSystem.WindScript.WindIntensity > 0.25f )
				_weather_type = WeatherType.WINDY;
			else if( WeatherSystem.FogScript != null && WeatherSystem.FogScript.FogDensity > 0.25f  )
				_weather_type = WeatherType.FOGGY;
			else
				_weather_type = WeatherType.CLEAR;

			return _weather_type;
		}
	}

#elif ICE_TENKOKU
	public class ICEWorldEnvironmentAdapter : ICEWorldEnvironment 
	{
		
		private TenkokuModule m_WeatherSystem = null;
		private TenkokuModule WeatherSystem{
			get{
				if( m_WeatherSystem == null )
					m_WeatherSystem = (TenkokuModule) FindObjectOfType(typeof(TenkokuModule));

				return m_WeatherSystem;
			}
		}

		private float m_UpdateTimer = 0;
		public float UpdateInterval = 10;

		public override void Awake (){
			UpdateWeather();
		}


		public override void Update (){

			m_UpdateTimer += Time.deltaTime;
			if( m_UpdateTimer < UpdateInterval )
				return;

			m_UpdateTimer = 0;

			UpdateWeather();


		}

		public void UpdateWeather()
		{
			if( WeatherSystem == null )
				Debug.LogWarning ("Sorry, TenkokuModule not exists!");

			WeatherForecast = ConvertWeatherType();
			//Temperature = WeatherSystem.temperature;
			DateDay = (int)WeatherSystem.currentDay;
			DateMonth = (int)WeatherSystem.currentMonth;
			DateYear = (int)WeatherSystem.currentYear;
			TimeHour = WeatherSystem.currentHour;
			TimeMinutes = WeatherSystem.currentMinute;
			TimeSeconds = WeatherSystem.currentSecond;

			WindSpeed = WeatherSystem.weather_WindAmt;
			WindDirection = WeatherSystem.weather_WindDir;

			UpdateTemperatureScale( TemperatureScaleType.FAHRENHEIT );

			Temperature = WeatherSystem.weather_temperature;
			MinTemperature = 0;
			MaxTemperature = 120;
		}

		private WeatherType ConvertWeatherType()
		{
			WeatherType _weather_type = WeatherType.UNDEFINED;

			if( WeatherSystem.weather_RainAmt > 0.5f || WeatherSystem.weather_SnowAmt > 0.5f )
				_weather_type = WeatherType.HEAVY_RAIN;
			else if( WeatherSystem.weather_RainAmt > 0.25f || WeatherSystem.weather_SnowAmt > 0.25f )
				_weather_type = WeatherType.RAIN;
			else if( WeatherSystem.weather_OvercastAmt > 0.5f )
				_weather_type = WeatherType.MOSTLY_CLOUDY;
			else if( WeatherSystem.weather_cloudCumulusAmt > 0.75f )
				_weather_type = WeatherType.CLOUDY;
			else if( WeatherSystem.weather_cloudCumulusAmt > 0.25f )
				_weather_type = WeatherType.PARTLY_CLOUDY;
			else if( WeatherSystem.weather_WindAmt > 0.50f )
				_weather_type = WeatherType.STORMY;
			else if( WeatherSystem.weather_WindAmt > 0.25f )
				_weather_type = WeatherType.WINDY;
			else if( WeatherSystem.weather_FogAmt > 0.25f )
				_weather_type = WeatherType.FOGGY;
			else
				_weather_type = WeatherType.CLEAR;

			return _weather_type;
		}
	}
#elif ICE_UNISTORM
	public class ICEWorldEnvironmentAdapter : ICEWorldEnvironment 
	{
		private UniStormWeatherSystem_C m_UniStormWeatherSystem = null;
		private UniStormWeatherSystem_C UniStormWeatherSystem{
			get{
				if( m_UniStormWeatherSystem == null )
				{
					GameObject _object = GameObject.Find( "UniStormSystemEditor" );
					if( _object != null )
						m_UniStormWeatherSystem = _object.GetComponent<UniStormWeatherSystem_C>();
				}

				return m_UniStormWeatherSystem;
			}
		}

		private float m_UpdateTimer = 0;
		public float UpdateInterval = 10;

		public int UniStormTemperature;
		public int UniStormWeatherType;
		public string UniStormWeather;
		public string UniStormTime;

		void Awake (){
			UpdateWeather();
		}


		void Update (){

			m_UpdateTimer += Time.deltaTime;
			if( m_UpdateTimer < UpdateInterval )
				return;
	
			m_UpdateTimer = 0;

			UpdateWeather();


		}

		public void UpdateWeather()
		{
			if( UniStormWeatherSystem == null )
				Debug.LogWarning ("Sorry, UniStormSystemEditor not exists!");
			
			UniStormWeatherType = (int)UniStormWeatherSystem.weatherForecaster;			
			UniStormTime = UniStormWeatherSystem.hourCounter.ToString () + ":" + UniStormWeatherSystem.minuteCounter.ToString () + " " + UniStormWeatherSystem.dayCounter.ToString () + "/" + UniStormWeatherSystem.monthCounter.ToString () + "/" + UniStormWeatherSystem.yearCounter.ToString();
			UniStormTemperature = UniStormWeatherSystem.temperature;

			WeatherForecast = ConvertWeatherType();
			Temperature = UniStormWeatherSystem.temperature;
			DateDay = (int)UniStormWeatherSystem.dayCounter;
			DateMonth = (int)UniStormWeatherSystem.monthCounter;
			DateYear = (int)UniStormWeatherSystem.yearCounter;
			TimeHour = UniStormWeatherSystem.hourCounter;
			TimeMinutes = UniStormWeatherSystem.minuteCounter;
			TimeSeconds = 0;

			if( UniStormWeatherSystem.temperatureType == 1 )
				UpdateTemperatureScale( TemperatureScaleType.FAHRENHEIT );
			else
				UpdateTemperatureScale(  TemperatureScaleType.CELSIUS );

			Temperature = UniStormTemperature;
			MinTemperature = UniStormWeatherSystem.minWinterTemp;
			MaxTemperature = UniStormWeatherSystem.maxSummerTemp;
		}

		private WeatherType ConvertWeatherType()
		{
			WeatherType _weather_type = WeatherType.UNDEFINED;

			switch( UniStormWeatherType )
			{
			case 1:
				UniStormWeather = "Foggy";
				_weather_type = WeatherType.FOGGY;
				break;
			case 2:
				UniStormWeather = "Rain or Snow";
				_weather_type = WeatherType.RAIN;
				break;
			case 3:
				UniStormWeather = "Stormy";
				_weather_type = WeatherType.STORMY;
				break;
			case 4:
				UniStormWeather = "Partly Cloudy";
				_weather_type = WeatherType.PARTLY_CLOUDY;
				break;
			case 5:
				UniStormWeather = "Partly Cloudy";
				_weather_type = WeatherType.PARTLY_CLOUDY;
				break;
			case 6:
				UniStormWeather = "Partly Cloudy";
				_weather_type = WeatherType.PARTLY_CLOUDY;
				break;
			case 7:
				UniStormWeather = "Clear";
				_weather_type = WeatherType.CLEAR;
				break;
			case 8:
				UniStormWeather = "Clear";
				_weather_type = WeatherType.CLEAR;
				break;
			case 9:
				UniStormWeather = "Cloudy";
				_weather_type = WeatherType.CLOUDY;
				break;
			case 10:
				UniStormWeather = "Butterflies";
				_weather_type = WeatherType.CLEAR;
				break;
			case 11:
				UniStormWeather = "Mostly Cloudy";
				_weather_type = WeatherType.MOSTLY_CLOUDY;
				break;
			case 12:
				UniStormWeather = "Heavy Rain";
				_weather_type = WeatherType.HEAVY_RAIN;
				break;
			case 13:
				UniStormWeather = "Falling Leaves";
				_weather_type = WeatherType.WINDY;
				break;
			default:
				UniStormWeather = "Undefined";
				_weather_type = WeatherType.UNDEFINED;
				break;

			}

			return _weather_type;
		}

	}
#else
	public class ICEWorldEnvironmentAdapter : ICEWorldEnvironment 
	{
	}
#endif
}
