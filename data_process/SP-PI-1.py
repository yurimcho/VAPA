import pymssql
import numpy as np
import pandas as pd
import math
import matplotlib as mpl
import matplotlib.pyplot as plt

'''
power : [kw] 계측마력
coeffA : [-] 변환계수
coeffB : [-] 변환계수
absWindSpeed : [m/s] 절대 풍속
absWindDir : [deg] 절대풍향
BN : [-] beaufort Number
airTemp : [deg] 기온
pressure : [hpa] 기압
speedVg_knot : [knot] 선박대지속도
shipHeading : [deg] 선박Heading
'''

class ShipDataProcessor:
    def ConvertPowerToSpeed(self, power, coeffA, coeffB):
        #시운전 시 계측된 선박 shaft power를 선박 speed로 변환(speed = A * power^B )
        result = coeffA * np.power(power,coeffB)
        return result # shaft power에 해당하는 선박 speed

    def ConvertPowerToSpeed(self, power, coefA, coefB):
        #시운전 시 계측된 선박 shaft power를 shaft power로 변환(rpm = A * power^B )
        result = coefA * np.power(power,coefB)
        return result # shaft power에 해당하는 shaft rpm
    
    def ConvertWindspeedToBN(self, absWindSpeed):
        #wind speed를 BN로 변환
        absWindSpeed = abs(absWindSpeed)
        if absWindSpeed < 0.3:
            result = 0
        elif absWindSpeed >= 0.3 and absWindSpeed < 1.5:
            result = 1
        elif absWindSpeed >= 1.5 and absWindSpeed < 3.3:
            result = 2
        elif absWindSpeed >= 3.3 and absWindSpeed < 5.5:
            result = 3
        elif absWindSpeed >= 5.5 and absWindSpeed < 7.9:
            result = 4
        elif absWindSpeed >= 7.9 and absWindSpeed < 10.7:
            result = 5
        elif absWindSpeed >= 10.7 and absWindSpeed < 13.8:
            result = 6
        elif absWindSpeed >= 13.8 and absWindSpeed < 17.1:
            result = 7
        elif absWindSpeed >= 17.1 and absWindSpeed < 20.7:
            result = 8
        elif absWindSpeed >= 20.7 and absWindSpeed < 24.4:
            result = 9
        elif absWindSpeed >= 24.4 and absWindSpeed < 28.4:
            result = 10
        elif absWindSpeed >= 28.4 and absWindSpeed < 32.6:
            result = 11
        elif absWindSpeed >= 32.6:
            result = 12
        return result

    def ConvertAirTempToAirDensity(self, airTemp, pressure):
        # 대기밀도
        absAirTemp = airTemp + 273.15
        pressure = pressure * 100
        result = pressure / (287.05 * absAirTemp)
        return result

    def ConvertRelWindToTrueWind(self, realWindSpeed, relWindDir, speedVg_knot, shipHeading):
        speedVg = speedVg_knot * 0.5144 # knots를 m/s로 변환
        #v_wr, phi_wr : relative wind speed / direction (form anemometet)
        #v_wt, phi_wt : true wind speed / ditection (from ISO19030 annex E)

        #relative wind를 truewind speed로 변환
        v_wr = realWindSpeed
        phi_wr = relWindDir
        vg = speedVg
        phi_o = shipHeading 

        v_wt = np.sqrt(np.power(v_wr, 2) + np.power(vg, 2) - 2 * v_wr * vg * np.cos(phi_wr * np.pi/180))
        v_wtbool = v_wr * np.cos(phi_wr*np.pi/180 + phi_o * np.pi/180) - vg * np.cos(phi_o * np.pi/180) 
        phi_wt = np.arctan((v_wr * np.sin(phi_wr * np.pi/180 + phi_o * np.pi/180) - vg*np.sin(phi_o * np.pi/180)) / (v_wr * np.cos(phi_wr * np.pi/180 + phi_o * np.pi/180) -  vg*np.sin(phi_o * np.pi/180))) 

        if v_wtbool < 0:
            phi_wt = phi_wt + (180 *np.pi/180)
        else:
            phi_wt = phi_wt
        return phi_wt

        # v_wr = vw - vg = v_wrx + v_wry
        v_wrx = v_wt * np.cos((270 - v_wt) * np.pi/180) - vg * np.cos((90 - phi_o) * np.pi/180)
        v_wry = v_wt * np.sin((270 - phi_wt) * np.pi/180) - vg * np.sin((90 - phi_o) * np.pi/180)
        v_wr = np.sqrt(np.power(v_wrx, 2) + np.power(v_wry, 2))
        phi_wr = - 90 - 180 * math.atan2(v_wry, v_wrx) / np.pi - phi_o * np.pi/180

        if phi_wr < -180:
            phi_wr = phi_wr + 360
        else:
            phi_wr = phi_wr  

        return phi_wr

    def IttcWindCoeff(self, shipType): 
        pass
        #수정중
        '''
        bulkDegree = np.array([0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180])
        bulkCoeff = np.array([-0.86962, -0.767089, -0.63038, -0.459494, -0.33038, -0.216456, -0.136709, -0.0759494, -0.0531646, 0, 0.0683544, 0.163291, 0.273418, 0.349367, 0.451899, 0.543038, 0.592405, 0.634177, 0.618987])
        # 선종별 계수 추가 예정
        if shipType == "BULK_CARRIER":
            result = np.polyfit(bulkDegree, bulkCoeff, 7)
        elif shipType == "CONTAINER":
            result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7)
        elif shipType == "CHEMICAL_CARRIER":
            result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7)
        else:
            result = curvefitting.polynominalRegression(bulkDegree, bulkCoeff, 7)
        
        return result
        '''

    def WindResistance(self, windCoeff, draft_ref, AT_ballast, breadth, draft, airTemp, airPressure, relWindSpeed, relWindDir, speedVg ):
        # 수정중...ing
        v_wr = relWindSpeed
        phi_wr = relWindDir
        vg = speedVg
        pass
        '''
        if v_wr > 180:
            v_wr = 360 - phi_wr # 상대풍속을 180도 이내로 제한 (커브피팅이 0 ~ 180도와 360 ~ 180 값이 동일)
        else:
            phi_wr = abs(phi_wr)
            airDensity = self.ConvertAirTempToAirDensity(20, 1013) # 날씨 데이터로 바꿀 것.!!!!!!!!
            A_XV = AT_ballast + (draft_ref - draft) * breadth # 수선상 트랜버스 면적을 드라프트 변화에 따라 계산
            coefRelWind = 0.5# (수정) windCoeff[7] * math.pow(relWindDir, 6) + windCoeff[6] * math.pow(relWindDir, 5) + windCoeff[5] * math.pow(relWindDir, 4) + windCoeff[4] * Math.Pow(relWindDir, 3) + windCoeff[3] * Math.Pow(relWindDir, 2) + windCoeff[2] * relWindDir + windCoeff[1]
            coefZeroWind = 0.7  #(수정) windCoeff[1] #WINDRESISTANCECoef[0] 값
            Rrw = 0.5 * airDensity * coefRelWind * A_XV * np.power(v_wr, 2) * -1
            R0w = 0.5 * airDensity * coefZeroWind * A_XV * np.power(vg * 0.5144, 2) * -1 # knot ---> m/s 주의하자
            deltaRw = ((Rrw - R0w) * vg * 0.5144) / 1000
		
        if (deltaRw) or (deltaRw):
            deltaRw = 0
            deltaRw = round(deltaRw, 2)

        return deltaRw
        '''

    def speedPowerTable(self):
        pass
    
    def speedPowerTableJson(self):
        pass  

    def GetDistanceGrateCircle(self):
        pass

    def SLIP(self):
        pass 

    def ConvertFocToPower(self):
        pass 

    def ConvertPowerToFoc(self):
        pass 

    def PVcalculator(self):
        pass 

    def PPVcalculator(self):
        pass 

    def ConvertPowerToFoc(self):
        pass 

class FocToPowerCalculation:
    def __init__(self, DCoeff, CCoeff, BCoeff, ACoeff):
    self._D = DCoeff
    self._C = CCoeff
    self._B = BCoeff
    self._A = ACoeff

    def Calculate(self, inputVariable):
        i = inputVariable
        focIndex_i = (self._D * i * i * i + self._C * i * i + self._B * i + self._A) * i / 1000 * 10200 / 9700
        return focIndex_i
        raise NotImplementedException()




if __name__ == '__main__':
    F = ShipDataProcessor()
    convertBN = F.ConvertWindspeedToBN(-20)
    convertDEN = F.ConvertAirTempToAirDensity(20,1000)
    convertTW = F.ConvertRelWindToTrueWind(20,150,10,30)
    #windCAA = F.IttcWindCoeff(BULK_CARRIER)
    windResist = F.WindResistance(0.5,10,11,20,10.5,10,100,10,100,15)
    print(convertBN, convertDEN, convertTW, windResist)

    C=FocToPowerCalculation(2,5,96,8)
    C.Calculate(5)
    print(C.Calculate(5))