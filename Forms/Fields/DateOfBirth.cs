using System;

namespace DUCK.Forms.Fields
{
	public struct DateOfBirth
	{
		public int year;
		public int month;
		public int day;

		public DateOfBirth(int year, int month, int day)
		{
			this.year = year;
			this.month = month;
			this.day = day;
		}

		public DateTime ToDateTime()
		{
			return new DateTime(year, month, day);
		}
	}
}