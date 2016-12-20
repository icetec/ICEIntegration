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
using System.Collections;

using ICE;
using ICE.World;
using ICE.World.EnumTypes;
using ICE.World.Utilities;
using ICE.World.Objects;

namespace ICE.Integration.Adapter
{
#if ICE_UNISTORM
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
