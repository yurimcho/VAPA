## writer : lee sang bong / lab021
import pymssql
import numpy as np
import pandas as pd

class Init():
    
    def __init__(self, callSign='3ewb4', beginDate='2017-01-01', endDate='2017-01-10'):
    
        """데이터베이스로 부터 데이터 요청함. 데이터 목록은 아래와 같음.
    
        LATITUDE, LONGITUDE, SPEED OVER GROUND, LONGITUDINAL GROUND SPEED, LONGITUDINAL WATER SPEED, HEADING GYRO, HEADING GPS, RUDDER ANGLE, SEA DEPTH,
        REL' WIND SPEED, REL' WIND DIRECTION, TRUE WIND SPEED, TRUE WIND DIRECTION, FWD' DRAFT, AFT' DRAFT, PROPELLER RPM, SHAFT TORQUE, SHAFT THRUST, SHAFT POWER, 
        SEAWATER TEMP, AIR_TEMP, AIR_PRESSURE, TRUE CURRENT SPEED, TRUE CURRENT DIRECTION, TOTAL WAVE HEIGHT, TOTAL WAVE DIRECTION, TOTAL WAVE PERIOD, SWELL WAVE HEIGHT, 
        SWELL WAVE DIRECTION, SWELL WAVE PERIOD, WIND WAVE HEIGHT, WIND WAVE DIRECTION, WIND WAVE PERIOD.
        Parameters
        ----------
        callSign : string, default='3ewb4'
            요청하고자 하는 선박의 Call Sign입니다.
        beginDate : string, default='2017-01-10'
            요청하고자 하는 시작날짜입니다.
        endDate : string, default='2017-02-10'
            요청하고자 하는 마지막 날짜입니다.
    
        Example
        ----------
        dbCon = sqlcon('3ewb4','2017-01-01','2017-02-01')
        shipData = dbCon.query(isShuffle=True, isPD= False, meanTime = 10)
        """
        self.callSign = callSign
        self.beginDate = beginDate
        self.endDate = endDate
       

    # ask for data from database
    def queryAll(self, isShuffle = False, isPD = True, meanTime = 0):

        """선박 운항 데이터 요청
        Parameters
        ----------
        isShuffle : boolean, default = "True"
            데이터를 무작위로 섞는 여부. True - 데이터를 섞음, False - 데이터를 섞지 않고 시간 순으로 정렬.
        isPD : boolean, default = "False"
            데이터 리턴은 PANDAS 형태로 가공할 것인지 여부. True - List 형태로 리턴, False - Pandas DataFrame 형태로 리턴.
        meanTime : int, default = 0
            데이터를 분단위로 normalizing 함. 예) meanTime = 10 - 10분 블록으로 평균을 냄.
        Returns
        -------
        DataFrame(Pandas) or array
            요청한 선박, 날짜에 해당하는 데이터.
        """
        conn = pymssql.connect(server='218.39.195.13:21000', user='sa', password='@120bal@', database='SHIP_DB_MARS')
    
        features = "[TIME_STAMP], [SPEED_VG], [SPEED_LW], [COURSE_OVER_GROUND], [SHIP_HEADING],[RUDDER_ANGLE], [WATER_DEPTH],\
        [REL_WIND_SPEED],[REL_WIND_DIR], [ABS_WIND_SPEED], [ABS_WIND_DIR],[DRAFT_FORE], [DRAFT_AFT], [SHAFT_REV],[SHAFT_TORQUE], [SHAFT_THRUST], [SHAFT_POWER], [BHP_BY_FOC],\
        [SST],[AIR_TEMP], [AIR_PRESSURE], [CURRENT_VEL],[CURRENT_DIR], [HTSGW], [MWSDIR],[MWSPER], [SWELL_SEQ1], [SWDIR_SEQ1], [SWDIR_SEQ1],[SWPER_SEQ1], [WVHGT], [WVDIR], [WVPER],[CURRENT_VEL_TAIL_TO_HEADING_NOWCAST]"

        stmt = "SELECT" + features + "FROM [SHIP_DB_MARS].[dbo].[SAILING_DATA] WHERE CALLSIGN ='"+self.callSign+"' AND TIME_STAMP > '"  + self.beginDate + "' AND TIME_STAMP < '"+ self.endDate +"' AND AT_SEA = 1 AND IS_GOOD_DATA_FOR_ANALYSIS = 1 ORDER BY TIME_STAMP"

        _data = pd.read_sql(stmt,conn)
        #row의 데이터 결측이 있을 시 해당 row 삭제

        # _data = df.dropna(axis=0)
        _data['SPEED_CURRENT'] = _data['SPEED_VG'] + _data['CURRENT_VEL_TAIL_TO_HEADING_NOWCAST']

        if  meanTime > 0 :
            _data = _data.resample(rule=str(meanTime)+'min', on='TIME_STAMP').mean()
            # _data = _data.dropna(axis=0)

        if isShuffle == True:
            result = _data.sample(n=_data.shape[0],random_state=0)
        else:
            result = _data.head(_data.shape[0])

        if isPD == True:
            result = result.loc[:]

        if isPD == False:
            result = (result.loc[:])
            result = result.values
        return result

    def QueryShipParticualr(self):
        """선박 재원 정보 요청
        Parameters
        ----------
        Returns
        -------
        DataFrame(Pandas) or array
            CALLSIGN : 선박 unique Key, 
            loa : length over all(m), 
            lbp : length between perpendicular(m), 
            ME_POWER_MCR : main engine max continuous rating Power(kw), 
            ME_RPM_MCR : main engine max continuous rating Rpm(rpm)
            PROPELLER_PITCH : propeller pitch(cm), 
            PROPELLER_DIAMETER : propeller diameter(cm),
            SPEED_etaD_BALLAST : speed at Ballast in Model Test(model test시 ballast에서 수행한 speed - etaD_BALLAST와 순서대로 짝이 맞음.)
            etaD_BALLAST : etaD at Ballast in Model Test(model test시 ballast에서 수행한 etaD = SPEED_etaD_BALLAST와 순서대로 짝이 맞음.)
            SPEED_etaD_SCANTLING : speed at Scantling in Model Test(model test시 scantling에서 수행한 speed - etaD_SCANTLING 순서대로 짝이 맞음.)
            etaD_SCANTLING : etaD at Scantling in Model Test(model test시 scantling 수행한 etaD = SPEED_etaD_SCANTLING 순서대로 짝이 맞음.)
            DRAFT_FORE_BALLAST : DRAFT FORE at Ballast in Model Test
            DRAFT_MID_BALLAST : DRAFT MID at Ballast in Model Test
            DRAFT_AFT_BALLAST : DRAFT AFT at Ballast in Model Test
            DRAFT_FORE_SCANTLING : DRAFT FORE at Ballast in Model Test
            DRAFT_MID_SCANTLING : DRAFT MID at Ballast in Model Test
            DRAFT_AFT_SCANTLING : DRAFT AFT at Ballast in Model Test
            ZREF : Reference height of Anemometer(m), In general, use of 10m (for calculating wind resistance)
            ZA_BALLAST : Height of Anemometer at Ballast(from waterline to an Anemometer at ballast)
            AOD : Lateral projected area of superstructures above upeerdeck(m2) (for calculating wind resistance)
            ALA : Lateral projected area above the waterline including superstructure (m2) (for calculating wind resistance)
            CMC : Horizontal distance from midship section to centre of lateral projected area ALA(m) (for calculating wind resistance)
            HBR : Height of top of superstructures from waterline(m) (for calculating wind resistance)
            HC : Height from waterline to centre of lateral projected area ALA(m) (for calculating wind resistance)
            MU : Smoothing range(Degree), Range of angles used for calculating wind resistance coefficient at 90degree. In general, use of 10degree (for calculating wind resistance)
            AXV : Transverse Projected Area above Waterline (m2) (for calculating wind resistance)
            AM_BALLAST : Lateral projected area under the waterline at Ballast(m2)
            KYY : Non-dimensional radius of gyration (y) In general, a range of value is 0.2 ~ 0.3
            CB_BALLAST : block coefficient at Ballast in Model Test
            CB_SCANTLING : block coefficient at Scantling in Model Test
            S_BALLAST : Surface area of hull under waterline at Ballast in Model Test(m2)
            S_SCANTLING : Surface area of hull under waterline at Scantling in Model Test(m2)
            DISPLACEMENT_BALLAST : Displacement at Ballast in Model Test(ton)
            DISPLACEMENT_SCANTLING : Displacement at Scantling in Model Test(ton)
            B_SPEED_TO_POWER_BALLAST : Coefficient B-Value(Speed to Power Curve - Power = B-coeff X SPEED ^ A-coeff) at Ballast in Sea-Trial
            A_SPEED_TO_POWER_BALLAST : Coefficient A-Value(Speed to Power Curve - Power = B-coeff X SPEED ^ A-coeff) at Ballast in Sea-Trial
            B_SPEED_TO_POWER_SCANTLING : Coefficient B-Value(Speed to Power Curve - Power = B-coeff X SPEED ^ A-coeff) at Scantling in Sea-Trial
            A_SPEED_TO_POWER_SCANTLING : Coefficient A-Value(Speed to Power Curve - Power = B-coeff X SPEED ^ A-coeff) at Scantling inSea-Trial
            WAKE_FACTOR_BALLAST : Wake factor at Ballast in Model Test
            WAKE_FACTOR_SCANTLING : Wake factor at SCANTLING in Model Test

            J = C_KQ_TO_J * KQ^2 + B_KQ_TO_J * KQ + C_KQ_TO_J 다음식으로 J을 구할 수 있음. 여기에서 KQ = torque / (waterDensity * (PROPELLAR_DIAMETER / 1000)^5 * (SHAFT_REV / 60)^2)

        """
        conn = pymssql.connect(server='218.39.195.13:21000', user='sa', password='@120bal@', database='SHIP_DB_MARS')
        stmt = "SELECT * FROM [SHIP_DB_MARS].[dbo].[SHIP_PARTICULAR_DETAIL] WHERE CALLSIGN ='"+self.callSign+"'"
        df = pd.read_sql(stmt,conn)
        return df
