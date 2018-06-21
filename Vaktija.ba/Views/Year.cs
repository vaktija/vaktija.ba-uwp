using System;
using System.Collections.Generic;
using Vaktija.ba.Helpers;

namespace Vaktija.ba.Views
{
    class Offset
    {
        public int month { get; set; }
        public int location_id { get; set; }
        public List<int> offsets { get; set; }
    }
    class Vakat
    {
        public int rbr { get; set; }
        public DateTime time { get; set; }
        public string name { get; set; }
        public bool alarmSkipped { get; set; }
        public bool toastSkipped { get; set; }
        public static string Key(Vakat v)
        {
            return (v.time.Year + "|" + v.time.Month + "|" + v.time.Day + "|" + v.name.Substring(0, 2).ToLower());
        }
    }
    class Day
    {
        public List<Vakat> vakti { get; set; }
    }
    class Month
    {
        public List<Day> days { get; set; }
    }
    class Year
    {
        public static Year year = new Year();
        public int locationID { get; set; }
        public List<Month> months { get; set; }

        public static void Set()
        {
            List<Offset> offset = Offset();
            List<Day> schedule = Schedule();

            Year y = new Year
            {
                locationID = Memory.location.id,
                months = new List<Month>(),
            };

            for (int a = 1; a <= 12; a++)
            {
                List<Day> days = new List<Day>();

                foreach (var it in schedule)
                {
                    if (it.vakti[0].time.Month == a)
                    {
                        List<Vakat> vakti = new List<Vakat>();
                        int br = 0;
                        foreach (var v in it.vakti)
                        {
                            Vakat vi = new Vakat
                            {
                                alarmSkipped = AlarmSkipping.IsSkipped(v),
                                name = v.name,
                                toastSkipped = ToastSkipping.IsSkipped(v),
                                time = v.time,
                                rbr = v.rbr,
                            };
                            vi.time = vi.time.AddMinutes(offset[a - 1].offsets[br]);
                            if (vi.rbr == 2 && Memory.Std_Podne)
                            {
                                if (!TimeZoneInfo.Local.IsDaylightSavingTime(vi.time))
                                    vi.time = new DateTime(vi.time.Year, vi.time.Month, vi.time.Day, 12, 0, 0);
                                else
                                    vi.time = new DateTime(vi.time.Year, vi.time.Month, vi.time.Day, 13, 0, 0);
                            }
                            vakti.Add(vi);
                            br++;
                        }
                        days.Add(new Day { vakti = vakti });
                    }
                }
                y.months.Add(new Month { days = days });
            }

            Year.year = y;
        }
        public static List<Offset> Offset()
        {
            List<Offset> offsets = new List<Offset>();

            for (int j = 1; j <= 12; j++)
            {
                if (Memory.location.id < 107)
                {
                    foreach (var it in Data.data.razlike)
                    {
                        if (it.grad_id == Memory.location.id && it.mjesec_id == j)
                        {
                            Offset tmp_offset = new Offset
                            {
                                month = j,
                                location_id = Memory.location.id,
                                offsets = new List<int> { it.zora, it.zora, it.podne, it.ikindija, it.ikindija, it.ikindija },
                            };
                            offsets.Add(tmp_offset);
                            break;
                        }
                    }
                }
                else if (Memory.location.id > 107)
                {
                    foreach (var it in Data.data.razlike_sandzak)
                    {
                        if (it.grad_id == Memory.location.id && it.mjesec_id == j)
                        {
                            Offset tmp_offset = new Offset
                            {
                                month = j,
                                location_id = Memory.location.id,
                                offsets = new List<int> { it.zora, it.zora, it.podne, it.ikindija, it.ikindija, it.ikindija },
                            };
                            offsets.Add(tmp_offset);
                            break;
                        }
                    }
                }
                else
                {
                    Offset tmp_offset = new Offset
                    {
                        month = j,
                        location_id = Memory.location.id,
                        offsets = new List<int> { 0, 0, 0, 0, 0, 0 },
                    };
                    offsets.Add(tmp_offset);
                }
            }
            return offsets;
        }
        public static List<Day> Schedule()
        {
            List<Day> days = new List<Day>();

            foreach (var it in Data.data.takvim)
            {
                List<DateTime> tempList = new List<DateTime>();
                tempList.Add(Convert.ToDateTime(it.datum + " " + it.zora));
                tempList.Add(Convert.ToDateTime(it.datum + " " + it.izlazaksunca));
                tempList.Add(Convert.ToDateTime(it.datum + " " + it.podne));
                tempList.Add(Convert.ToDateTime(it.datum + " " + it.ikindija));
                tempList.Add(Convert.ToDateTime(it.datum + " " + it.aksam));
                tempList.Add(Convert.ToDateTime(it.datum + " " + it.jacija));

                List<Vakat> vakti = new List<Vakat>();

                for (int i = 0; i < tempList.Count; i++)
                {
                    Vakat vakat = new Vakat
                    {
                        rbr = i,
                        alarmSkipped = false,
                        toastSkipped = false,
                        time = tempList[i],
                        name = Fixed.timeNames[i],
                    };
                    //if (vakat.time.DayOfWeek == DayOfWeek.Friday) vakat.name = "Podne (Džuma)";
                    vakti.Add(vakat);
                }
                days.Add(new Day { vakti = vakti });
            }
            return days;
        }
    }
}