using System;

namespace DUCK.Forms.Fields
{
	public struct DateOfBirth
	{
		public int day;
		public int month;
		public int year;

		public DateOfBirth(int day, int month, int year)
		{
			this.day = day;
			this.month = month;
			this.year = year;
		}

		public DateTime ToDateTime()
		{
			return new DateTime(year, month, day);
		}
	}
}