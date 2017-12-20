﻿using System;
using UnityEngine;

namespace DUCK.Forms
{
	public static class DateOfBirthUtils
	{
		public static int[] Days { get; private set; }
		public static int[] Years { get; private set; }
		public static string[] Months { get; private set; }

		public enum Month
		{
			January = 1,
			Febuary,
			March,
			April,
			May,
			June,
			July,
			August,
			September,
			October,
			November,
			December
		}

		static DateOfBirthUtils()
		{
			const int days = 31;
			Days = new int[days];
			for (var i = 0; i < days; i++)
			{
				Days[i] = i + 1;
			}

			Months = Enum.GetNames(typeof(Month));

			const int years = 100;
			var currentYear = DateTime.Now.Year;
			Years = new int[years];
			for (var i = 0; i < years; i++)
			{
				Years[i] = currentYear - i;
			}
		}
	}
}