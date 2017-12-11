using SPMS_BACKEND.Models.ShipModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPMS_BACKEND.Methods.ShipData
{
    public class ISO19030Convert
    {
        static public List<SAILING_DATA_FOR_19030> Convert(List<SAILING_DATA> sailingData, SHIP_PARTICULAR_DETAIL shipParticularDetail)
        {
            List<SAILING_DATA_FOR_19030TEMP> rawShipdata = new List<SAILING_DATA_FOR_19030TEMP>();

            foreach (var item in sailingData)
            {
                SAILING_DATA_FOR_19030TEMP temp = new SAILING_DATA_FOR_19030TEMP();
                temp.CALLSIGN = item.CALLSIGN;
                temp.TIME_STAMP = item.TIME_STAMP;
                temp.SPEED_VG = item.SPEED_VG;
                temp.SPEED_LW = item.SPEED_LW;
                temp.SPEED_LW_VIRTUAL = item.SPEED_LW_VIRTUAL;
                temp.REL_WIND_DIR = item.REL_WIND_DIR;
                temp.REL_WIND_SPEED = item.REL_WIND_SPEED;
                temp.WIND_SPEED_HEADING_TO_TAIL = item.WIND_SPEED_HEADING_TO_TAIL;
                temp.COURSE_OVER_GROUND = item.COURSE_OVER_GROUND;
                temp.SHIP_HEADING = item.SHIP_HEADING;
                temp.WATER_DEPTH = item.WATER_DEPTH;
                temp.RUDDER_ANGLE = item.RUDDER_ANGLE;
                temp.SW_TEMP = item.SW_TEMP;
                temp.AIR_TEMP = item.AIR_TEMP;
                temp.AIR_DENSITY = item.AIR_DENSITY;
                temp.AIR_PRESSURE = item.AIR_PRESSURE;
                temp.DRAFT_FORE = item.DRAFT_FORE;
                temp.DRAFT_AFT = item.DRAFT_AFT;

                temp.ROLL = item.ROLL;
                temp.PITCH = item.PITCH;

                temp.ABS_WIND_DIR = item.ABS_WIND_DIR;
                temp.ABS_WIND_SPEED = item.ABS_WIND_SPEED;
                temp.BHP_BY_FOC = item.BHP_BY_FOC;
                temp.ME_FOC_HOUR = item.ME_FOC_HOUR;
                temp.SLIP = item.SLIP;
                temp.SHAFT_REV = item.SHAFT_REV;
                temp.SHAFT_POWER = item.SHAFT_POWER;
                temp.SHAFT_THRUST = item.SHAFT_THRUST;
                temp.SHAFT_TORQUE = item.SHAFT_TORQUE;
                temp.SSS = item.SSS;
                temp.SST = item.SST;
                temp.CURRENT_UV = item.CURRENT_UV;
                temp.CURRENT_VV = item.CURRENT_VV;
                temp.CURRENT_VEL = item.CURRENT_VEL;
                temp.CURRENT_DIR = item.CURRENT_DIR;
                temp.CURRENT_VEL_TAIL_TO_HEADING_NOWCAST = item.CURRENT_VEL_TAIL_TO_HEADING_NOWCAST;
                temp.SWDIR_SEQ1 = item.SWDIR_SEQ1;
                temp.SWDIR_SEQ2 = item.SWDIR_SEQ2;
                temp.WVDIR = item.WVDIR;
                temp.MWSPER = item.MWSPER;
                temp.MWSDIR = item.MWSDIR;
                temp.SWPER_SEQ1 = item.SWPER_SEQ1;
                temp.SWPER_SEQ2 = item.SWPER_SEQ2;
                temp.WVPER = item.WVPER;
                temp.DIRPW = item.DIRPW;
                temp.PERPW = item.PERPW;
                temp.HTSGW = item.HTSGW;
                temp.SWELL_SEQ1 = item.SWELL_SEQ1;
                temp.SWELL_SEQ2 = item.SWELL_SEQ2;
                temp.WVHGT = item.WVHGT;
                temp.UGRD = item.UGRD;
                temp.VGRD = item.VGRD;
                temp.WDIR = item.WDIR;
                temp.WIND = item.WIND;
                temp.WIND_SPEED_HEADING_TO_TAIL_NOWCAST = item.WIND_SPEED_HEADING_TO_TAIL_NOWCAST;
                temp.VALID_CHAUVENT = true;
                temp.VALID_VALIDATION = true;
                temp.VALID_REFCONDITION = true;
                rawShipdata.Add(temp);
            }

            List<SAILING_DATA_FOR_19030TEMP> afterChauvent_shipdata = new List<SAILING_DATA_FOR_19030TEMP> { };       //인수로 넣은 List 와 동일한 자료형으로 만들 것
            List<SAILING_DATA_FOR_19030TEMP> afterValID_shipdata = new List<SAILING_DATA_FOR_19030TEMP> { };
            List<SAILING_DATA_FOR_19030TEMP> afterRef_shipdata = new List<SAILING_DATA_FOR_19030TEMP> { };

            var callSign = shipParticularDetail.CALLSIGN;

            int FILTER_TOTAL_COUNT = 0;
            int VALID_COUNT = 0;
            int REF_CONDITION_COUNT = 0;
            int count_rawShipdata = 0;

            foreach (var item in rawShipdata)
            {
                item.ID = count_rawShipdata;
                count_rawShipdata++;
            }

            IEnumerable<List<SAILING_DATA_FOR_19030TEMP>> tenMinBlock_shipdata =
            from s in rawShipdata
            group s by new
            {
                s.TIME_STAMP.Year,
                s.TIME_STAMP.Month,
                s.TIME_STAMP.Day,
                s.TIME_STAMP.Hour,
                Minute = s.TIME_STAMP.Minute / 10
            } into groupShipdata
            orderby groupShipdata.Key.Year, groupShipdata.Key.Month, groupShipdata.Key.Day, groupShipdata.Key.Hour, groupShipdata.Key.Minute ascending

            select
            groupShipdata.ToList();

            foreach (var item in tenMinBlock_shipdata)
            {
                var BHP_BY_FOC = Chauvent(item.Select(d => d.BHP_BY_FOC));
                var REL_WIND_DIR = Chauvent_angle(item.Select(d => d.REL_WIND_DIR));
                var REL_WIND_SPEED = Chauvent(item.Select(d => d.REL_WIND_SPEED));
                var SPEED_VG = Chauvent(item.Select(d => d.SPEED_VG));
                var SHAFT_REV = Chauvent(item.Select(d => d.SHAFT_REV));
                var SHIP_HEADING = Chauvent_angle(item.Select(d => d.SHIP_HEADING));
                var DRAFT_FORE = Chauvent(item.Select(d => d.DRAFT_FORE));
                var DRAFT_AFT = Chauvent(item.Select(d => d.DRAFT_AFT));
                var WATER_DEPTH = Chauvent(item.Select(d => d.WATER_DEPTH));
                var RUDDER_ANGLE = Chauvent_angle(item.Select(d => d.RUDDER_ANGLE));
                var SW_TEMP = Chauvent(item.Select(d => d.SST));
                var AIR_TEMP = Chauvent(item.Select(d => d.AIR_TEMP));
                var SHAFT_POWER = Chauvent(item.Select(d => d.SHAFT_POWER));

                int count_chauvent = 0;

                foreach (var item2 in item)
                {
                    item2.VALID_CHAUVENT = BHP_BY_FOC[count_chauvent] && REL_WIND_DIR[count_chauvent] && REL_WIND_SPEED[count_chauvent] && SPEED_VG[count_chauvent] && SHAFT_REV[count_chauvent] &&
                                            SHIP_HEADING[count_chauvent] && DRAFT_FORE[count_chauvent] && DRAFT_AFT[count_chauvent] && WATER_DEPTH[count_chauvent] && RUDDER_ANGLE[count_chauvent] &&
                                            SW_TEMP[count_chauvent] && AIR_TEMP[count_chauvent] && SHAFT_POWER[count_chauvent];

                    if (item2.VALID_CHAUVENT == true)
                    {
                        afterChauvent_shipdata.Add(item2);
                    }
                    else
                    {
                        rawShipdata[FILTER_TOTAL_COUNT].VALID_CHAUVENT = false;
                        rawShipdata[FILTER_TOTAL_COUNT].VALID_VALIDATION = false;
                        rawShipdata[FILTER_TOTAL_COUNT].VALID_REFCONDITION = false;
                    }

                    FILTER_TOTAL_COUNT++;
                    count_chauvent++;
                }
            }

            IEnumerable<List<SAILING_DATA_FOR_19030TEMP>> afterChauvent_tenMinBlock_shipdata =
            from s in afterChauvent_shipdata
            group s by new
            {
                s.TIME_STAMP.Year,
                s.TIME_STAMP.Month,
                s.TIME_STAMP.Day,
                s.TIME_STAMP.Hour,
                Minute = s.TIME_STAMP.Minute / 10
            } into groupShipdata
            orderby groupShipdata.Key.Year, groupShipdata.Key.Month, groupShipdata.Key.Day, groupShipdata.Key.Hour, groupShipdata.Key.Minute ascending

            select
            groupShipdata.ToList();

            foreach (var item in afterChauvent_tenMinBlock_shipdata)
            {
                var RUDDER_ANGLE_VALID = standardError_angle(item.Select(d => d.RUDDER_ANGLE), 1);
                var SHAFT_REV_VALID = standardError(item.Select(d => d.SHAFT_REV), 3);
                var SPEED_VG_VALID = standardError(item.Select(d => d.SPEED_VG), 0.5f);

                int temp_i = 0;

                foreach (var item2 in item)
                {
                    int index = (int)item2.ID;

                    item2.VALID_VALIDATION = SHAFT_REV_VALID[temp_i] && SPEED_VG_VALID[temp_i] && RUDDER_ANGLE_VALID[temp_i];

                    if (item2.VALID_VALIDATION == true)
                    {
                        afterValID_shipdata.Add(item2);
                    }
                    else
                    {
                        rawShipdata.ElementAt((int)item2.ID).VALID_VALIDATION = false;
                        rawShipdata.ElementAt((int)item2.ID).VALID_REFCONDITION = false;
                        VALID_COUNT++;
                    }
                }
            }

            foreach (var item in afterValID_shipdata)
            {
                var speedVg = item.SPEED_VG * 0.5144f;
                var waterDepth1 = 3 * Math.Sqrt(shipParticularDetail.BREADTH * ((item.DRAFT_FORE + item.DRAFT_AFT) / 2));
                var waterDepth2 = 2.75 * (speedVg * speedVg) / 9.80665;
                if (waterDepth1 < waterDepth2)
                {
                    waterDepth1 = waterDepth2;
                }

                int index = (int)item.ID;
                if (item.SST <= 2 || item.SST > 50 || item.WATER_DEPTH <= waterDepth1 || Math.Abs(item.RUDDER_ANGLE) >= 5)
                {
                    rawShipdata.ElementAt(index).VALID_REFCONDITION = false;
                    REF_CONDITION_COUNT++;
                }
                else
                {
                    afterRef_shipdata.Add(item);
                    rawShipdata.ElementAt(index).VALID_REFCONDITION = true;
                }
            }

            List<SAILING_DATA_FOR_19030> result = new List<SAILING_DATA_FOR_19030>();
            foreach (var item in rawShipdata)
            {
                if (item.VALID_CHAUVENT == false || item.VALID_CHAUVENT == false || item.VALID_REFCONDITION == false)
                {
                    SAILING_DATA_FOR_19030 temp = new SAILING_DATA_FOR_19030();
                    temp.CALLSIGN = item.CALLSIGN;
                    temp.TIME_STAMP = item.TIME_STAMP;
                    temp.SPEED_VG = item.SPEED_VG;
                    temp.SPEED_LW = item.SPEED_LW;
                    temp.SPEED_LW_VIRTUAL = item.SPEED_LW_VIRTUAL;
                    temp.REL_WIND_DIR = item.REL_WIND_DIR;
                    temp.REL_WIND_SPEED = item.REL_WIND_SPEED;
                    temp.WIND_SPEED_HEADING_TO_TAIL = item.WIND_SPEED_HEADING_TO_TAIL;
                    temp.COURSE_OVER_GROUND = item.COURSE_OVER_GROUND;
                    temp.SHIP_HEADING = item.SHIP_HEADING;
                    temp.WATER_DEPTH = item.WATER_DEPTH;
                    temp.RUDDER_ANGLE = item.RUDDER_ANGLE;
                    temp.SW_TEMP = item.SW_TEMP;
                    temp.AIR_TEMP = item.AIR_TEMP;
                    temp.AIR_DENSITY = item.AIR_DENSITY;
                    temp.AIR_PRESSURE = item.AIR_PRESSURE;
                    temp.DRAFT_FORE = item.DRAFT_FORE;
                    temp.DRAFT_AFT = item.DRAFT_AFT;

                    temp.ROLL = item.ROLL;
                    temp.PITCH = item.PITCH;


                    temp.ABS_WIND_DIR = item.ABS_WIND_DIR;
                    temp.ABS_WIND_SPEED = item.ABS_WIND_SPEED;
                    temp.BHP_BY_FOC = item.BHP_BY_FOC;
                    temp.ME_FOC_HOUR = item.ME_FOC_HOUR;
                    temp.SLIP = item.SLIP;
                    temp.SHAFT_REV = item.SHAFT_REV;
                    temp.SHAFT_POWER = item.SHAFT_POWER;
                    temp.SHAFT_TORQUE = item.SHAFT_TORQUE;
                    temp.SHAFT_THRUST = item.SHAFT_THRUST;
                    temp.SSS = item.SSS;
                    temp.SST = item.SST;
                    temp.CURRENT_UV = item.CURRENT_UV;
                    temp.CURRENT_VV = item.CURRENT_VV;
                    temp.CURRENT_VEL = item.CURRENT_VEL;
                    temp.CURRENT_DIR = item.CURRENT_DIR;
                    temp.CURRENT_VEL_TAIL_TO_HEADING_NOWCAST = item.CURRENT_VEL_TAIL_TO_HEADING_NOWCAST;
                    temp.SWDIR_SEQ1 = item.SWDIR_SEQ1;
                    temp.SWDIR_SEQ2 = item.SWDIR_SEQ2;
                    temp.WVDIR = item.WVDIR;
                    temp.MWSPER = item.MWSPER;
                    temp.MWSDIR = item.MWSDIR;
                    temp.SWPER_SEQ1 = item.SWPER_SEQ1;
                    temp.SWPER_SEQ2 = item.SWPER_SEQ2;
                    temp.WVPER = item.WVPER;
                    temp.DIRPW = item.DIRPW;
                    temp.PERPW = item.PERPW;
                    temp.HTSGW = item.HTSGW;
                    temp.SWELL_SEQ1 = item.SWELL_SEQ1;
                    temp.SWELL_SEQ2 = item.SWELL_SEQ2;
                    temp.WVHGT = item.WVHGT;
                    temp.UGRD = item.UGRD;
                    temp.VGRD = item.VGRD;
                    temp.WDIR = item.WDIR;
                    temp.WIND = item.WIND;
                    temp.WIND_SPEED_HEADING_TO_TAIL_NOWCAST = item.WIND_SPEED_HEADING_TO_TAIL_NOWCAST;
                    result.Add(temp);
                }
            }

            return result;
        }

        static public bool[] Chauvent(IEnumerable<float> values)       // 10분 블록 데이터 묶음을 인수로 넣음
        {
            int count = values.Count();
            double[] debug = new double[count];     // 디버그용 최종 Chauvent 값을 넣음
            bool[] valID = new bool[count];
            double avg = 0;
            double StdDev = 0;
            if (count > 1)
            {
                avg = values.Average();       // 10 분 블록 평균 계산

                double delta = values.Sum(d => Math.Pow((d - avg), 2));        // 중간 계산 -> (value-avg)^2

                StdDev = Math.Sqrt(delta / count);        // 표준 편차 계산
            }

            int i = 0;

            if (StdDev == 0)                // 표준 편차가 0 일 경우 true를 찍기 위한 코드 (모든 값이 같으면 표준편차가 0)
            {
                foreach (var item in values)
                {
                    valID[i] = true;
                    i++;
                }
            }
            else
            {
                foreach (var item in values)
                {
                    debug[i] = MathNet.Numerics.SpecialFunctions.Erfc(Math.Abs((item - avg) / (StdDev * Math.Sqrt(2)))) * count;                    // 디버깅용 코드
                    valID[i] = MathNet.Numerics.SpecialFunctions.Erfc(Math.Abs((item - avg) / (StdDev * Math.Sqrt(2)))) * count < 0.5 ? false : true;
                    i++;
                }
            }
            return valID;
        }

        static public bool[] Chauvent_angle(IEnumerable<float> values)
        {
            double StdDev = 0;
            int count = values.Count();
            double sintotal = 0;
            double costotal = 0;
            double[] deltai = new double[count];
            double[] debug = new double[count];
            bool[] valID = new bool[count];
            double avg = 0;
            List<double> values_corr = new List<double> { };

            foreach (var item in values)
            {
                if (item < 0)
                {
                    values_corr.Add(item + 360);
                }
                else
                {
                    values_corr.Add(item);
                }
            }

            if (count > 1)
            {
                sintotal = values_corr.Sum(d => Math.Sin(d / (180 / Math.PI)));          // sin 값 합계
                costotal = values_corr.Sum(d => Math.Cos(d / (180 / Math.PI)));          // cos 값 합계
                avg = Math.Atan2(sintotal, costotal);                             //블록 평균 계산 Atan2 (y, x) parameter 로 넣는다. public static double Atan2(double y, double x)
                avg = avg * (180 / Math.PI);
                double delta = 0;

                int temp_i = 0;
                foreach (var item in values_corr)
                {
                    var r = (Math.Abs(item - avg) % 360);

                    if ((r) > 180)
                    {
                        deltai[temp_i] = (360 - r);

                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    else
                    {
                        deltai[temp_i] = r;

                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    temp_i++;
                }

                StdDev = Math.Sqrt(delta / count);        //표준 편차 계산

                if (StdDev == 0)                          // StdDev 가 0 일 경우 - 한 블록안에 값이 모두 같다면 StdDev 0 이나 이때 true가 되어야 함.
                {
                    for (int i = 0; i < count; i++)
                    {
                        valID[i] = true;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        //debug[i] = MathNet.Numerics.SpecialFunctions.Erfc(deltai[i] / (StdDev * Math.Sqrt(2))) * count;           // 디버깅 코드
                        valID[i] = MathNet.Numerics.SpecialFunctions.Erfc(deltai[i] / (StdDev * Math.Sqrt(2))) * count < 0.5 ? false : true;
                    }
                }
            }
            return valID;
        }

        static public bool[] standardError(IEnumerable<float> values, float threshold)
        {
            int count = values.Count();
            double[] debug = new double[count];
            bool[] valID = new bool[count];
            double avg = 0;
            double StdDev = 0;
            if (count > 1)
            {
                avg = values.Average();       //블록 평균 계산

                double delta = values.Sum(d => (d - avg) * (d - avg));        //중간 계산 -> (value-avg)^2

                StdDev = Math.Sqrt(delta / count);        //표준 편차 계산
            }

            for (int i = 0; i < count; i++)
            {
                valID[i] = StdDev > threshold ? false : true;
            }

            return valID;
        }

        static public bool[] standardError_angle(IEnumerable<float> values, float threshold)
        {
            double StdDev = 0;
            int count = values.Count();
            double sintotal = 0;
            double costotal = 0;
            double[] deltai = new double[count];
            bool[] valID = new bool[count];
            double avg = 0;

            if (count > 1)
            {
                sintotal = values.Sum(d => Math.Sin(d / (180 / Math.PI)));          // 180 / Math.PI 각도를 라디안으로 변환
                costotal = values.Sum(d => Math.Cos(d / (180 / Math.PI)));
                avg = Math.Atan2(sintotal, costotal);      //블록 평균 계산 Atan2 (y, x) parameter 로 간다. public static double Atan2(double y, double x)

                avg = avg * (180 / Math.PI);

                double delta = 0;

                int temp_i = 0;
                foreach (var item in values)
                {
                    var r = (Math.Abs(item - avg) % 360);

                    if ((r) > 180)
                    {
                        deltai[temp_i] = (360 - r);
                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    else
                    {
                        deltai[temp_i] = r;
                        delta = delta + Math.Pow(deltai[temp_i], 2);
                    }
                    temp_i++;
                }

                StdDev = Math.Sqrt(delta / count);        //표준 편차 계산

                for (int i = 0; i < count; i++)
                {
                    valID[i] = StdDev > threshold ? false : true;
                }
            }
            return valID;
        }
    }
}