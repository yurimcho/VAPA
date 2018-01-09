#-*-coding: utf-8
""" ISO19030 
    5.3 Data Preparation 
    5.3.4 data filtering and validation
     1) make 10minute data blocks
     2) Chauvent criteriton filter (Annex I)
     3) Standard Error of mean validation (Annex J)
-----------------
주)
1. 10분 block에 대한 4가지 filter를 거는 코드를 만들었습니다.
현재... 걸러진 data외 정상 data가 반환되지 않아,,, 원인을 찾고 있습니다.
2. StandaradError filter에서 모든 data가 filter에서 false로 인식되고 있어서 STDE filter를 확인하고 있습니다.
Anlge로 정의되어 있는 data filter의 오류로 생각되어..수정하고 있습니다.
"""


import numpy as np
import pandas as pd
import sys
import os
import math
sys.path.append(os.path.join(sys.path[0],'data_process'))
sys.path.append(os.path.join(sys.path[0],'data_analysis'))
sys.path.append(os.path.join(sys.path[0],'helper'))
import datain as di
import dataout as do

# 10분 datablock 정의
class Datablock: 
    def __init__(self, Chauvent, Chauvent_angle, StandardError, StandardError_angle):    
        self.Cha = Chauvent
        self.Cha_A = Chauvent_angle
        self.StdE = StandardError
        self.StdE_A = StandardError_angle

    # values : dataframe 전체
    # field name : filter 할 필드 이름
    def run(self, values):
        sTime = values.iloc[0,0] # 시작시간 가져옴
        t = sTime 
        tenM = pd.Timedelta('10min') # 10분 delta time으로...
        tenBlock = values[df.TIME_STAMP < sTime + tenM] # 조건 인덱싱으로 첫 10분 간격의 데이터 추출

        resultBlock = pd.DataFrame(columns=values.columns, index=values.index) # filtering 되지 않은 data 저장 
        #resultBlock = pd.DataFrame
        #resultBlock = None

        i = 0
        while len(tenBlock) > 0: #data수가 0보다 많으면 계산 수행   
            #Chauvenet's filter 
                   
            M = self.Cha.Chauvent(tenBlock['SPEED_VG'] ) # DataFilter class 내의 Chauvent def. 실행
            M &= self.Cha.Chauvent(tenBlock['SPEED_LW']) # Longitudinal Water Speed    
            M &= self.Cha.Chauvent(tenBlock['SHAFT_POWER']) # from shaft power meter[kW]
            M &= self.Cha.Chauvent(tenBlock['SHAFT_REV']) # from shaft power meter[rpm]
            M &= self.Cha.Chauvent(tenBlock['SHAFT_TORQUE']) # from shaft power meter[kNm]    
            M &= self.Cha.Chauvent(tenBlock['BHP_BY_FOC']) # from annexC(연료소모량 계산)
            M &= self.Cha.Chauvent(tenBlock['REL_WIND_SPEED']) # from anemometer 
            #M &= self.Cha_A.Chauvent_Angle(tenBlock['REL_WIND_DIR']) # from anemometer
            #M &= self.Cha_A.Chauvent_Angle(tenBlock['SHIP_HEADING'])
            #M &= self.Cha_A.Chauvent_Angle(tenBlock['RUDDER_ANGLE'])
            M &= self.Cha.Chauvent(tenBlock['WATER_DEPTH'])
            M &= self.Cha.Chauvent(tenBlock['DRAFT_FORE'])
            M &= self.Cha.Chauvent(tenBlock['DRAFT_AFT'])
            M &= self.Cha.Chauvent(tenBlock['SST']) # Seawater Temp.
            M &= self.Cha.Chauvent(tenBlock['AIR_TEMP'])
        
            # Validation filter
            M &= self.StdE.StandardError(tenBlock['SHAFT_REV'], 3)
            M &= self.StdE.StandardError(tenBlock['SPEED_VG'], 0.5)
            M &= self.StdE.StandardError(tenBlock['SPEED_LW'], 0.5)
            #M &= self.StdE_A.StandardError_angle(tenBlock['RUDDER_ANGLE'], 1)
        
            i += 1
            print(M)
            if M.all():     
                resultBlock.append(tenBlock) 
            else:
                #pass
                print('filtered')
                

            t += tenM  # 다음 10분 block 수행
            tenBlock = df[(t <= df.TIME_STAMP) & (df.TIME_STAMP < t + tenM)] # 첫 10분 block과 마찮가지로 조건인덱싱으로 추출

        #resultBlock.dropna(0)
        #print(resultBlock.head(10))
        print('{} vLen : {}, result len : {}, M len : {}'.format(i, len(values), len(resultBlock), len(M)))
        resultBlock.to_csv("resultBlock.csv") # filtering된 data csv로 저장


# Filter 정의
class DataFilter:
    def Chauvent(self, values):  #DataFrame이 아닌, seriese value로 받아서 filtering
        mean = values.mean()  #평균        
        delta = values.apply(lambda x : abs(x - mean))  #delta = abs(values - mean), 각각 행을 계산(lambda이용)
        N = len(values)  #변수 갯수
        sigma = math.sqrt((1/N)*sum(delta**2))  #standard error of the mean        

        if sigma !=0:
            di = delta.apply(lambda x : x/(sigma*math.sqrt(2)))       
            retrv = di.apply(lambda x : math.erfc(x)*N >= 0.5)  #math.erfc(x)*N < 0.5 ? false : true
        else:
            #retrv = pd.Series([True])
            retrv = True

        return retrv
        

    def Chauvent_Angle(self, values):  #DataFrame이 아닌, seriese value로 받아서 filtering
        mean_sin = math.sin((values/(180/math.pi)).mean())  # sin 평균 
        mean_cos = math.cos((values/(180/math.pi)).mean())  # cos 평균
        mean = math.atan2(mean_cos, mean_sin)
        mean = mean * (180/math.pi) 

        delta = values.apply(lambda x : abs(x - mean))
        N = len(values)  #변수 갯수
        sigma = math.sqrt((1/N)*sum(delta**2))  #standard error of the mean

        if sigma !=0:
            di = delta.apply(lambda x : x/(sigma*math.sqrt(2)))       
            retrv = di.apply(lambda x : math.erfc(x)*N >= 0.5)  #math.erfc(x)*N < 0.5 ? false : true
        else:
            #retrv = pd.Series([True])
            retrv = True      

        return retrv


    def StandardError(self, values, StdE): # 'SHAFT_REV'(RPM), 'SPEED_VG', 'SPEED_LW'
        mean = values.mean()  #평균        
        delta = values.apply(lambda x : abs(x - mean))  #delta = abs(values - mean), 각각 행을 계산(lambda이용)
        N = len(values)  #변수 갯수
        retrv = values.apply(lambda x :math.sqrt((1/N)*sum(delta**2)) < StdE) 
    
        return retrv
        

    def StandardError_angle(self, values, StdE): # 'RUDDER_ANGLE'
        mean_sin = math.sin((values/(180/math.pi)).mean())  # sin 평균 
        mean_cos = math.cos((values/(180/math.pi)).mean())  # cos 평균
        mean = math.atan2(mean_cos, mean_sin)
        mean = mean * (180/math.pi) 

        delta = values.apply(lambda x : abs(x - mean))

        N = len(values)  #변수 갯수
        retrv = values.apply(lambda x :math.sqrt((1/N)*sum(delta**2)) < StdE) 

        return retrv
    



# 다른 파일에서 본 파일을 부르면 실행 되지 않고, 행당 파일에서만 실행 되는 부분
if __name__ == '__main__':
    # data 준비
    # 1. data 불러오기
    datain = di.Init('3FFB8','2017-02-22','2017-02-25') # server에서 불러오기(하루 data임. 소장님께서 비교 항차기간 결정후 수정 예정)
    df = datain.queryAll(isShuffle=False, isPD=True, meanTime = 0)
    '''
    df = pd.read_csv('sample.csv') # csv로 부르기
    del df['Unnamed: 0']
    df['TIME_STAMP'] = pd.to_datetime( df['TIME_STAMP'] )
'''
    # 2. data 정렬(혹시, sorting이 안되어 있을수도 있으니...시간순으로 오름차순 sorting)
    df = df.sort_values(["TIME_STAMP"], ascending = [True]) # ascending = [True] : 오름차순 / [False] : 내림차순
    #print(df.head(5)) # 정렬확인
    

    f = DataFilter() #인스턴스 정의, DataFilter class실행
    datablock = Datablock(f,f,f,f) # Datablock class init 실행 (값->DataFilter의 함수 받음) 
    datablock.run(df) # datablock의 run def 실행



