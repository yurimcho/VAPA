using Newtonsoft.Json.Linq;
using SPMS_BACKEND.Models.ShipModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TridentGoalSeek;

namespace SPMS_BACKEND.Methods.ShipData
{
    public class ShipDataProcessor
    {
        /// <summary>
        /// 시운전 시 계측된 선박 축마력을 스피드로 변환 (speed = coeffA * power ^ coeffB)
        /// </summary>
        /// <param name="power">계측마력(KW)</param>
        /// <param name="coeffA">변환계수 A</param>
        /// <param name="coeffB">변환계수 B</param>
        /// <returns>축마력에 해당하는 선박속도</returns>
        public float ConvertPowerToSpeed(float power, float coeffA, float coeffB)         // 상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            float result = (float)(coeffA * Math.Pow(power, coeffB));
            return result;
        }

        /// <summary>
        /// 시운전 시 계측된 선박 축마력을 축회전수로 변환 (rpm = coeffA * power ^ coeffB)
        /// </summary>
        /// <param name="power">계측마력(Kw)</param>
        /// <param name="coefA">변환계수 A</param>
        /// <param name="coefB">변환계수 B</param>
        /// <returns>축마력에 해당하는 축회전수</returns>
        public float ConvertPowerToRpm(float power, float coefA, float coefB)         // 상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            float result = (float)(coefA * Math.Pow(power, coefB));
            return result;
        }

        /// <summary>
        ///  절대 WindSpeed값을 Beaufort Number값으로 변환한다.
        /// </summary>
        /// <param name="absWindSpeed">@절대 풍속 값</param>
        /// <returns>Beaufort Numner</returns>
        public int ConvertWindspeedToBN(double absWindSpeed)         // 상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            int result = 0;

            if (absWindSpeed <= -0.3 && absWindSpeed > -1.5) result = -1;
            else if (absWindSpeed <= -1.5 && absWindSpeed < -3.3) result = -2;
            else if (absWindSpeed <= -3.3 && absWindSpeed > -5.5) result = -3;
            else if (absWindSpeed <= -5.5 && absWindSpeed > -7.9) result = -4;
            else if (absWindSpeed <= -7.9 && absWindSpeed > -10.7) result = -5;
            else if (absWindSpeed <= -10.7 && absWindSpeed > -13.8) result = -6;
            else if (absWindSpeed <= -13.8 && absWindSpeed > -17.1) result = -7;
            else if (absWindSpeed <= -17.1 && absWindSpeed > -20.7) result = -8;
            else if (absWindSpeed <= -20.7 && absWindSpeed > -24.4) result = -9;
            else if (absWindSpeed <= -24.4 && absWindSpeed > -28.4) result = -10;
            else if (absWindSpeed <= -28.4 && absWindSpeed > -32.6) result = -11;
            else if (absWindSpeed <= -32.6) result = -12;
            else if (absWindSpeed >= -0.3 && absWindSpeed < 0.3) result = 0;
            else if (absWindSpeed >= 0.3 && absWindSpeed < 1.5) result = 1;
            else if (absWindSpeed >= 1.5 && absWindSpeed < 3.3) result = 2;
            else if (absWindSpeed >= 3.3 && absWindSpeed < 5.5) result = 3;
            else if (absWindSpeed >= 5.5 && absWindSpeed < 7.9) result = 4;
            else if (absWindSpeed >= 7.9 && absWindSpeed < 10.7) result = 5;
            else if (absWindSpeed >= 10.7 && absWindSpeed < 13.8) result = 6;
            else if (absWindSpeed >= 13.8 && absWindSpeed < 17.1) result = 7;
            else if (absWindSpeed >= 17.1 && absWindSpeed < 20.7) result = 8;
            else if (absWindSpeed >= 20.7 && absWindSpeed < 24.4) result = 9;
            else if (absWindSpeed >= 24.4 && absWindSpeed < 28.4) result = 10;
            else if (absWindSpeed >= 28.4 && absWindSpeed < 32.6) result = 11;
            else if (absWindSpeed >= 32.6) result = 12;

            return result;
        }

        /// <summary>
        /// 대기 밀도를 구함
        /// </summary>
        /// <param name="airTemp">상대풍속(m/s)</param>
        /// <param name="pressure">상대풍향(degree)</param>
        /// <returns>대기 밀도</returns>
        public double ConvertAirTempToAirDensity(double airTemp, double pressure)         // 상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            double absAirTemp = airTemp + 273.15f;
            pressure = pressure * 100f;

            double result = pressure / (287.05f * absAirTemp);
            return result;
        }



        /// <summary>
        /// 상대풍속,풍향을 절대풍속,풍향으로 변환
        /// </summary>
        /// <param name="relWindSpeed">상대풍속(m/s)</param>
        /// <param name="relWindDir">상대풍향(degree)</param>
        /// <param name="speedVg_knot">선박대지속도(knot)</param>
        /// <param name="shipHeading">선박Heading(degree)</param>
        /// <returns>[0] 절대풍속, [1] 절대풍향</returns>
        public double[] ConvertRelWindToAbsWind(double relWindSpeed, double relWindDir, double speedVg_knot, double shipHeading)         // 상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            double[] absWind = new double[2];
            double speedVg = speedVg_knot * 0.5144;         // 선박의 속도를 knot에서 m/s로 변환, 풍속은 앞쪽에서 일괄 변환하였음.

            double REL_WIND_DIR_Rad = relWindDir * Math.PI / 180;
            double shipHeading_Rad = shipHeading * Math.PI / 180;

            absWind[0] = Math.Sqrt(Math.Pow(relWindSpeed, 2) + Math.Pow(speedVg, 2) - 2 * relWindSpeed * speedVg * Math.Cos(REL_WIND_DIR_Rad));       // 상대 풍속을 절대 풍속으로 변환식

            if ((relWindSpeed * Math.Cos(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Cos(shipHeading_Rad)) >= 0)         // 상대 풍향을 절대 풍향으로 변환
            {
                absWind[1] = (Math.Atan((relWindSpeed * Math.Sin(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Sin(shipHeading_Rad)) / (relWindSpeed * Math.Cos(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Cos(shipHeading_Rad))) * 180 / Math.PI);
            }
            else
            {
                absWind[1] = Math.Atan((relWindSpeed * Math.Sin(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Sin(shipHeading_Rad)) / (relWindSpeed * Math.Cos(REL_WIND_DIR_Rad + shipHeading_Rad) - speedVg * Math.Cos(shipHeading_Rad))) * 180 / Math.PI + 180;
            }

            if (double.IsNaN(absWind[0]))
            {
                absWind[0] = 0;
            }

            if (double.IsNaN(absWind[1]))
            {
                absWind[1] = 0;
            }

            if (absWind[1] < 0)
            {
                absWind[1] = absWind[1] + 360;
            }
            return absWind;
        }

        /// <returns>[0] 절대풍속, [1] 절대풍향</returns>

        /// <summary>
        /// 절대풍속,풍향을 상대풍속,풍향으로 변환
        /// </summary>
        /// <param name="absWindSpeed">절대풍속(m/s)</param>
        /// <param name="absWindDir">절대풍향(degree)</param>
        /// <param name="speedVg_knot">선박대지속도(knot)</param>
        /// <param name="shipHeading">선박Heading(degree)</param>
        /// <returns></returns>
        public double[] ConvertAbsWindToRelWind(double absWindSpeed, double absWindDir, double speedVg_knot, double shipHeading)         // 절대 풍향 풍속 --> 상대 풍향 풍속으로 변환하는 메소드
        {
            double[] relWind = new double[2];
            double speedVg = speedVg_knot * 0.5144;         // 선박의 속도를 knot에서 m/s로 변환, 풍속은 앞쪽에서 일괄 변환하였음.

            var v_WRx = absWindSpeed * Math.Cos((270 - absWindDir) * Math.PI / 180) - speedVg * Math.Cos((90 - shipHeading) * Math.PI / 180);
            var v_WRy = absWindSpeed * Math.Sin((270 - absWindDir) * Math.PI / 180) - speedVg * Math.Sin((90 - shipHeading) * Math.PI / 180);
            var v_WR = Math.Sqrt(v_WRx * v_WRx + v_WRy * v_WRy);
            var phi_WR = -90 - 180 * Math.Atan2(v_WRy, v_WRx) / Math.PI - shipHeading;
            phi_WR = phi_WR - 360 * Convert.ToInt16(phi_WR / 360);

            relWind[0] = v_WR;
            relWind[1] = phi_WR;

            if (double.IsNaN(v_WR))
            {
                relWind[0] = 0;
            }

            if (double.IsNaN(phi_WR))
            {
                relWind[1] = 0;
            }

            if (phi_WR < 0)
            {
                relWind[1] = phi_WR + 360;
            }
            return relWind;
        }
        


        /// <summary>
        /// 선박 WindResistance 계산 - ISO15016,ISO19030
        /// </summary>
        /// <param name="airDensityCoef">공기밀도계수 - 온도에 따라 값 결정</param>
        /// <param name="windCoeff">바람저항계수 - 풍행에 따라 값 결정</param>
        /// <param name="draft_ref">기준 draft 값 - draft에 따른 수선상 선박 넓이 계산</param>
        /// <param name="AT_ballast">ballast시 수선상 선박 넓이</param>
        /// <param name="breadth">선박넓이</param>
        /// <param name="draft">현재 draft 값</param>
        /// <param name="airTemp">공기 온도</param>
        /// <param name="relWindSpeed">상대 풍속(m/s)</param>
        /// <param name="relWindDir">상대 풍향(m/s)</param>
        /// <param name="speedVg">선박대지속도(knot)</param>
        /// <param name="etaD0"></param>
        /// <param name="etaDM"></param>
        /// <returns></returns>
        public double resistanceWind(SHIP_PARTICULAR shipParticular, double[] windCoeff, double draft_ref, double AT_ballast, double breadth, double draft, double airTemp, double airPressure, double relWindSpeed, double relWindDir, double speedVg)
        {
            if (relWindDir > 180)
            {
                relWindDir = 360 - relWindDir;      // 상대풍속을 180도 이내로 제한 (커브피팅이 0 ~ 180도와 360 ~ 180 값이 동일)
            }
            else if (relWindDir < 0)
            {
                relWindDir = Math.Abs(relWindDir);
            }

            var airDensity = this.ConvertAirTempToAirDensity(20, 1013);  // 날씨 데이터로 바꿀 것.!!!!!!!!

            double A_XV = AT_ballast + (draft_ref - draft) * breadth;      // 수선상 트랜버스 면적을 드라프트 변화에 따라 계산

            double coefRelWind = windCoeff[7] * Math.Pow(relWindDir, 6) + windCoeff[6] * Math.Pow(relWindDir, 5) + windCoeff[5] * Math.Pow(relWindDir, 4) + windCoeff[4] * Math.Pow(relWindDir, 3) + windCoeff[3] * Math.Pow(relWindDir, 2) + windCoeff[2] * relWindDir + windCoeff[1];

            double coefZeroWind = windCoeff[1]; //WINDRESISTANCECoef[0] 값

            double Rrw = 0.5 * airDensity * coefRelWind * A_XV * Math.Pow(relWindSpeed, 2) * -1;

            double R0w = 0.5 * airDensity * coefZeroWind * A_XV * Math.Pow(speedVg * 0.5144, 2) * -1;            // knot ---> m/s 주의하자

            double deltaRw = ((Rrw - R0w) * speedVg * 0.5144) / 1000;

            if (double.IsNaN(deltaRw) || double.IsInfinity(deltaRw))
            {
                deltaRw = 0;
            }
            deltaRw = Math.Round(deltaRw, 2);
            return deltaRw;         // 풍향이 마이너스로 나온다 확인할거
        }

        public double[] IttcWindCoeff(string shipType)         //상대 풍향 풍속 --> 절대 풍향 풍속으로 변환하는 메소드
        {
            var curvefitting = new CurveFitting();
            double[] bulkDegree = new double[] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180};
            double[] bulkCoeff = new double[] { -0.86962, -0.767089, -0.63038, -0.459494, -0.33038, -0.216456, -0.136709, -0.0759494, -0.0531646, 0, 0.0683544, 0.163291, 0.273418, 0.349367, 0.451899, 0.543038, 0.592405, 0.634177, 0.618987 };

            double[] result;

            switch (shipType)
            {
                case "BULK_CARRIER":
                    result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7);
                    break;
                case "CONTAINER":
                    result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7);
                    break;
                case "CHEMICAL_CARRIER":
                    result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7);
                    break;
                default:
                    result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7);
                    break;
            }
            return result;

        }





        /// <summary>
        /// 시운전 그래프를 기준으로 ballast와 scanlt 사이의 각 draft 별 power, speed 계수를 구함.
        /// </summary>
        /// <param name="shipParticular">선박제원정보</param>
        /// <param name="seaTrialPowerToSpeedAtBallast">Power Speed 변환계수(ballast)</param>
        /// <param name="seaTrialPowerToSpeedAtScant">Power Speed 변환계수(scant)</param>
        /// <returns>각 10cm 별 power speed 계수 배열</returns>
        public List<double[]> speedPowerTable(SHIP_PARTICULAR_DETAIL shipParticularDetail, SHIP_PARTICULAR_DETAIL standardData)
        {

            double seatrialdraftScantling = (standardData.DRAFT_FORE_SCANTLING + standardData.DRAFT_AFT_SCANTLING) * 0.5;  //Draft (Scantling) Fore , Draft (Scantling) Aft
            double seaTrialdraftBallast = (standardData.DRAFT_FORE_BALLAST + standardData.DRAFT_AFT_BALLAST) * 0.5;     //Draft (Ballast) Fore, Draft (Ballast) Aft

            List<double[]> draftTable = new List<double[]> { };
            double startDraft = Math.Truncate((seaTrialdraftBallast - 3) * 10);        // ref draft -2 부터 시작 (-2는 여유분)

            int numerOfDraft = (int)((Math.Truncate((seatrialdraftScantling + 3) * 10)) - (Math.Truncate((seaTrialdraftBallast - 3) * 10)));         // 스캔틀링까지 소수점 한자리 까지 검색, 기준 draft보다 -2 +2 만큼 큼
            int numberOfPwer = (int)(shipParticularDetail.ME_POWER_MCR / 200);          //ME_POWER_MCr
            double[] powerTable = new double[numberOfPwer];         // 확인 완  200 씩 파워 간격을 띄움 --> 커브피팅 알고리즘으로 x 축 배열
            double[] speedTable = new double[numberOfPwer];         // 확인 완  파워 간격 만큼 스피드를 계산 --> 커브피팅 알고리즘으로 y 축 배열
            var curveFitting = new CurveFitting();

            for (int i = 0; i < numerOfDraft; i++)          // 드라프트 간격 0.1 만큼 해상도의 커프피팅 테이블을 만듦
            {
                for (int j = 0; j < numberOfPwer; j++)
                {
                    powerTable[j] = (j + 1) * 200;
                    speedTable[j] =
                    ((standardData.B_SPEED_TO_POWER_SCANTLING * Math.Pow(powerTable[j], standardData.A_SPEED_TO_POWER_SCANTLING) - standardData.B_SPEED_TO_POWER_BALLAST * Math.Pow(powerTable[j], standardData.A_SPEED_TO_POWER_BALLAST)) /  // Seatrial PowerToSpeed(Scant')
                    (seatrialdraftScantling - seaTrialdraftBallast) *
                    ((i + startDraft) * 0.1 - seaTrialdraftBallast) + (standardData.B_SPEED_TO_POWER_BALLAST * Math.Pow(powerTable[j], standardData.A_SPEED_TO_POWER_BALLAST)));  // Seatrial PowerToSpeed(Ballast')
                }

                var curve = curveFitting.powerRegression(powerTable, speedTable);
                draftTable.Add(curve);
            }

            return draftTable;
        }

        /// <summary>
        /// 시운전 그래프를 기준으로 ballast와 scanlt 사이의 각 draft 별 power, speed 계수를 구하여 Json으로 리턴
        /// </summary>
        /// <param name="shipParticular">선박제원정보</param>
        /// <param name="seaTrialPowerToSpeedAtBallast">Power Speed 변환계수(ballast)</param>
        /// <param name="seaTrialPowerToSpeedAtScant">Power Speed 변환계수(scant)</param>
        /// <returns>각 10cm 별 power speed 계수를 Json 형식으로 변환</returns>

        public JObject speedPowerTableJson(SHIP_PARTICULAR_DETAIL shipParticularDetail, SHIP_PARTICULAR_DETAIL standardData)
        {
            JObject result = new JObject();
            double seatrialdraftScantling = (standardData.DRAFT_FORE_SCANTLING + standardData.DRAFT_AFT_SCANTLING) * 0.5;  //Draft (Scantling) Fore , Draft (Scantling) Aft
            double seaTrialdraftBallast = (standardData.DRAFT_FORE_BALLAST + standardData.DRAFT_AFT_BALLAST) * 0.5;     //Draft (Ballast) Fore, Draft (Ballast) Aft
            List<double[]> draftTable = new List<double[]> { };
            double startDraft = Math.Truncate((seaTrialdraftBallast - 3) * 10);        // ref draft -2 부터 시작 (-2는 여유분)
            int numerOfDraft = (int)((Math.Truncate((seatrialdraftScantling + 3) * 10)) - (Math.Truncate((seaTrialdraftBallast - 3) * 10)));         // 스캔틀링까지 소수점 한자리 까지 검색, 기준 draft보다 -2 +2 만큼 큼
            int numberOfPwer = (int)(shipParticularDetail.ME_POWER_MCR / 200);          //ME_POWER_MCr
            double[] powerTable = new double[numberOfPwer];         // 확인 완  200 씩 파워 간격을 띄움 --> 커브피팅 알고리즘으로 x 축 배열
            double[] speedTable = new double[numberOfPwer];         // 확인 완  파워 간격 만큼 스피드를 계산 --> 커브피팅 알고리즘으로 y 축 배열
            var curveFitting = new CurveFitting();

            for (int i = 0; i < numerOfDraft; i++)          // 드라프트 간격 0.1 만큼 해상도의 커프피팅 테이블을 만듦
            {
                for (int j = 0; j < numberOfPwer; j++)
                {
                    powerTable[j] = (j + 1) * 200;
                    speedTable[j] =
                    ((standardData.B_SPEED_TO_POWER_SCANTLING * Math.Pow(powerTable[j], standardData.A_SPEED_TO_POWER_SCANTLING) - standardData.B_SPEED_TO_POWER_BALLAST * Math.Pow(powerTable[j], standardData.A_SPEED_TO_POWER_BALLAST)) /  // Seatrial PowerToSpeed(Scant')
                    (seatrialdraftScantling - seaTrialdraftBallast) *
                    ((i + startDraft) * 0.1 - seaTrialdraftBallast) + (standardData.B_SPEED_TO_POWER_BALLAST * Math.Pow(powerTable[j], standardData.A_SPEED_TO_POWER_BALLAST)));  // Seatrial PowerToSpeed(Ballast')
                }
                var curve = curveFitting.powerRegression(speedTable, powerTable);
                result.Add(((startDraft + i) / 10).ToString(), new JArray(curve[1], curve[2]));
            }
            return result;
        }

        /// <summary>
        /// 대권 거리 구한다.
        /// </summary>
        /// <param name="lat1">시작 lat</param>
        /// <param name="lon1">시작 lon</param>
        /// <param name="lat2">도착 lat</param>
        /// <param name="lon2">도착 lon</param>
        /// <param name="unit">반환하는 거리의 유닛 K - Km, N - Knot</param>
        /// <returns></returns>
        /// 

        public double GetDistanceGrateCircle(double[] firstLatLong, double[] secondLatLong)
        {
            // 단위 Km임 .. 단위 선택하도록 할것.
            // 더블에 lat lon 순서로 들어감.
            if (firstLatLong.Length == 1)
            {
                double sum = 0;
                sum += System.Math.Abs(firstLatLong[0] - secondLatLong[0]);
                return sum;
            }
            double dLat = this.toRadian(secondLatLong[0] - firstLatLong[0]);
            double dLon = this.toRadian(secondLatLong[1] - firstLatLong[1]);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(this.toRadian(firstLatLong[0])) * Math.Cos(this.toRadian(secondLatLong[0])) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = 6371 * 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return d;
        }

        private double toRadian(double val)
        {
            return (Math.PI / 180) * val;
        }

        public double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            lat1 = lat1 / 100;
            lon1 = lon1 / 100;
            lat2 = lat2 / 100;
            lon2 = lon2 / 100;

            double degLat1;
            double minLat1;
            double degLat2;
            double minLat2;
            double degLon1;
            double minLon1;
            double degLon2;
            double minLon2;

            if (lat1 < 0)
            {
                lat1 = lat1 * -1;
                degLat1 = Math.Floor(lat1);
                minLat1 = (lat1 - degLat1) / 60 * 100;
                lat1 = (degLat1 + minLat1) * -1;
            }
            else
            {
                degLat1 = Math.Floor(lat1);
                minLat1 = (lat1 - degLat1) / 60 * 100;
                lat1 = (degLat1 + minLat1);
            }

            if (lat2 < 0)
            {
                lat2 = lat2 * -1;
                degLat2 = Math.Floor(lat2);
                minLat2 = (lat2 - degLat2) / 60 * 100;
                lat2 = (degLat2 + minLat2) * -1;
            }
            else
            {
                degLat2 = Math.Floor(lat2);
                minLat2 = (lat2 - degLat2) / 60 * 100;
                lat2 = (degLat2 + minLat2);
            }

            if (lon1 < 0)
            {
                lon1 = lon1 * -1;
                degLon1 = Math.Floor(lon1);
                minLon1 = (lon1 - degLon1) / 60 * 100;
                lon1 = (degLon1 + minLon1) * -1;
            }
            else
            {
                degLon1 = Math.Floor(lon1);
                minLon1 = (lon1 - degLon1) / 60 * 100;
                lon1 = (degLon1 + minLon1);
            }

            if (lon2 < 0)
            {
                lon2 = lon2 * -1;
                degLon2 = Math.Floor(lon2);
                minLon2 = (lon2 - degLon2) / 60 * 100;
                lon2 = (degLon2 + minLon2) * -1;
            }
            else
            {
                degLon2 = Math.Floor(lon2);
                minLon2 = (lon2 - degLon2) / 60 * 100;
                lon2 = (degLon2 + minLon2);
            }

            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }

            if (double.IsNaN(dist) || double.IsNegativeInfinity(dist) || double.IsPositiveInfinity(dist))
            {
                dist = 0;
            }

            return (dist);
        }

        /// <summary>
        /// degree 값를 radian으로 변환
        /// </summary>
        /// <param name="deg">degree 값</param>
        /// <returns>해당 degree의 radian 값</returns>
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        /// <summary>
        /// radian값을 degree로 변환
        /// </summary>
        /// <param name="rad">radian 값</param>
        /// <returns>해당 radian의 degree 값</returns>
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        /// <summary>
        /// 선속 추진 시 겉보기 slip를 구한다.
        /// </summary>
        /// <param name="shaftRev">추진기 rpm</param>
        /// <param name="speedVg">선박속도(knot)</param>
        /// <param name="propellerPitch">프로펠러피치</param>
        /// <returns>선박 slip %</returns>
        public double SLIP(double shaftRev, double speedVg, double propellerPitch)
        {
            double SLIP = -9999;
            if (shaftRev <= 0 || speedVg <= 0)
            {
                return SLIP;
            }
            else
            {
                SLIP = (1 - (speedVg / (shaftRev * 60 * propellerPitch / 1852 / 1000))) * 100;
            }

            if (double.IsNaN(SLIP) || double.IsPositiveInfinity(SLIP) || double.IsNegativeInfinity(SLIP))
            {
                SLIP = -9999;
            }

            Math.Round(SLIP, 4);

            return SLIP;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ME_FOC_HOUR"></param>
        /// <param name="shopTestPowerToSfoc"></param>
        /// <param name="mcr"></param>
        /// <param name="LCV"></param>
        /// <param name="shopTestLCV"></param>
        /// <returns></returns>
        public double ConvertFocToPower(double ME_FOC_HOUR, SHIP_SHOPTEST_DATA shopTestPowerToSfoc, float mcr, double LCV = 9700, double shopTestLCV = 10200)        // 연료사용량을 마력으로 환산 mass 연료사용량이 파라미터로 들어감.
        {

            if (ME_FOC_HOUR < 0)
            {
                return -9999;
            }

            double SHAFT_POWER_FOC = 0;
            var focToPowerCalculation = new FocToPowerCalculation(shopTestPowerToSfoc.D, shopTestPowerToSfoc.C, shopTestPowerToSfoc.B, shopTestPowerToSfoc.A);
            var goalSeeker = new GoalSeek(focToPowerCalculation);
            var seekResult = goalSeeker.SeekResult((decimal)ME_FOC_HOUR);

            double d = shopTestPowerToSfoc.D;
            double c = shopTestPowerToSfoc.C;
            double b = shopTestPowerToSfoc.B;
            double a = shopTestPowerToSfoc.A;

            //for (int i = 0; i < mcr + 2000; i += 1000)
            //{
            //    var focIndex_i = (d * Math.Pow(i, 3) + c * Math.Pow(i, 2) + b * i + a) * i / 1000 * shopTestLCV / LCV;

            //    if (focIndex_i > ME_FOC_HOUR)
            //    {
            //        for (int j = i - 1000; j < i; j += 100)
            //        {
            //            var focIndex_j = (d * Math.Pow(j, 3) + c * Math.Pow(j, 2) + b * j + a) * j / 1000 * shopTestLCV / LCV;

            //            if (focIndex_j > ME_FOC_HOUR)
            //            {
            //                for (int k = j - 100; k < j; k++)
            //                {
            //                    var focIndex_k = (d * Math.Pow(k, 3) + c * Math.Pow(k, 2) + b * k + a) * k / 1000 * shopTestLCV / LCV;

            //                    if (focIndex_k > ME_FOC_HOUR)
            //                    {
            //                        SHAFT_POWER_FOC = k;
            //                        return SHAFT_POWER_FOC;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            if (double.IsNaN(SHAFT_POWER_FOC) || SHAFT_POWER_FOC > mcr * 1.2 || SHAFT_POWER_FOC < 0)
            {
                return -9999;
            }

            SHAFT_POWER_FOC = Math.Round(SHAFT_POWER_FOC, 2);

            return SHAFT_POWER_FOC;
        }

        /// <summary>
        /// 마력을 일일연료소모량으로 환산
        /// </summary>
        /// <param name="powerToSfoc_D"></param>
        /// <param name="powerToSfoc_C"></param>
        /// <param name="powerToSfoc_B"></param>
        /// <param name="powerToSfoc_A"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public double ConvertPowerToFoc(double powerToSfoc_D, double powerToSfoc_C, double powerToSfoc_B, double powerToSfoc_A, double power)
        {
            var Power = power;
            var Sfoc = powerToSfoc_D * Math.Pow(Power, 3) + powerToSfoc_C * Math.Pow(Power, 2) + powerToSfoc_B * Power + powerToSfoc_A;
            double foc = Power * Sfoc / 1000 * 10200 / 9700;
            return foc;
        }

        public double PVcalculator(double[] speedPowerTable, double CORRECTPOW, double SPEED_LW, double draftfore, double draftaft)
        {
            double PV = 0;

            if (CORRECTPOW <= 0 | SPEED_LW <= 0)
            {
                return -9999;
            }



            var expectedSpeed = speedPowerTable[1] * Math.Pow(CORRECTPOW, speedPowerTable[2]);

            PV = 100 * (SPEED_LW - expectedSpeed) / expectedSpeed;

            if (double.IsNaN(PV) || double.IsNegativeInfinity(PV) || double.IsPositiveInfinity(PV))
            {
                PV = -9999;
            }

            return PV;
        }

        public double PPVcalculator(double[] speedPowerTable, double CORRECTPOW, double SPEED_LW, double draftfore, double draftaft)
        {
            double PPV = 0;

            if (CORRECTPOW <= 0 | SPEED_LW < 0)
            {
                return -9999;
            }

            var powerToSpeedCoef1 = Math.Pow(1 / speedPowerTable[1], 1 / speedPowerTable[2]);
            var powerToSpeedCoef2 = 1 / speedPowerTable[2];

            var expectedPower = powerToSpeedCoef1 * Math.Pow(SPEED_LW, powerToSpeedCoef2);

            PPV = 100 * (CORRECTPOW - expectedPower) / expectedPower;

            if (double.IsNaN(PPV) || double.IsNegativeInfinity(PPV) || double.IsPositiveInfinity(PPV))
            {
                PPV = -9999;
            }

            return PPV;
        }

        public bool[] standardError(IEnumerable<float> values, float threshold)
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

        public bool[] AverageError(IEnumerable<float> values, float threshold)
        {
            int count = values.Count();
            bool[] valID = new bool[count];
            double avg = 0;
            if (count > 1)
            {
                avg = values.Average();       //블록 평균 계산
            }

            for (int i = 0; i < count; i++)
            {
                valID[i] = avg > threshold ? false : true;
            }

            return valID;
        }




            
        public float AngleAverage(IEnumerable<float> values)
        {

           var sintotal = values.Sum(d => Math.Sin(d / (180 / Math.PI)));          // 180 / Math.PI 각도를 라디안으로 변환
          var  costotal = values.Sum(d => Math.Cos(d / (180 / Math.PI)));
          var  avg = Math.Atan2(sintotal, costotal);      //블록 평균 계산 Atan2 (y, x) parameter 로 간다. public static double Atan2(double y, double x)

            avg = avg * (180 / Math.PI);


            return (float)avg;
        }

        public double linearInterpolation(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

    }
    public class FocToPowerCalculation : IGoalSeekAlgorithm
    {
        decimal D;
        decimal C;
        decimal B;
        decimal A;

        public FocToPowerCalculation(double DCoeff, double CCoeff, double BCoeff, double ACoeff)
        {
            this.D = (decimal)DCoeff;
            this.C = (decimal)CCoeff;
            this.B = (decimal)BCoeff;
            this.A = (decimal)ACoeff;
        }

        public decimal Calculate(decimal inputVariable)
        {
           

            var i = inputVariable;
        
            var focIndex_i = (this.D * i*i*i + this.C * i*i + this.B * i + this.A) * i / 1000m * 10200m / 9700m;

            return focIndex_i;
            throw new NotImplementedException();
        }

    }
}