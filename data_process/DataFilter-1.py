#-*-coding: utf-8
""" ISO19030 
    5.3 Data Preparation 
    5.3.4 data filtering and validation
     1) make 10minute data blocks
     2) Chauvent criteriton filter (Annex I)
     3) Standard Error of mean validation (Annex J)
"""

import numpy as np
import pandas as pd
import sys
import os
import math
from time import strftime 
sys.path.append(os.path.join(sys.path[0],'data_process'))
sys.path.append(os.path.join(sys.path[0],'data_analysis'))
sys.path.append(os.path.join(sys.path[0],'helper'))
import datain as di
import dataout as do

start = pd.to_datetime(strftime("%y%m%d-%H%M%S"))


# 10분 datablock 정의
class Datablock: 
    def __init__(self, Chauvent, Chauvent_angle, StandardError, StandardError_angle):    
        self.Cha = Chauvent
        self.Cha_A = Chauvent_angle
        self.StdE = StandardError
        self.StdE_A = StandardError_angle

    # values : dataframe 전체
    def run(self, values):
        #df = pd.DataFrame(values)
        sTime = values.iloc[0,0] # 시작시간 가져옴
        t = sTime 
        tenM = pd.Timedelta('10min') # 10분 delta time으로...
        tenBlock = values[values.TIME_STAMP < sTime + tenM] # 조건 인덱싱으로 첫 10분 간격의 데이터 추출

        resultBlock = pd.DataFrame() # 결과 값을 저장할 임시 블럭 생성 
        filteredBlock = pd.DataFrame()

        #resultDf = pd.DataFrame(values, columns=['SPEED_VG', 'VG_Chav', 'SPEED_LW', 'LW_Chav'])
        #vg_chav = np.array()
        #lw_chav = np.array()


        i = 0
        while len(tenBlock) > 0: #data수가 0보다 많으면 계산 수행               

            #Chauvenet's filter                                    
            
            M = self.Cha.Chauvent(np.array(tenBlock['SPEED_VG'] )) # DataFilter class 내의 Chauvent def. 실행
            #vg_chav = np.append(vg_chav, self.Cha.Chauvent(tenBlock['SPEED_VG'] ))
            #print(type(M))
            M &= self.Cha.Chauvent(np.array(tenBlock['SHAFT_POWER'])) # from shaft power meter[kW]
            
            #M &= self.Cha.Chauvent(np.array(tenBlock['SHAFT_REV'])) # from shaft power meter[rpm]
            #M &= self.Cha.Chauvent(np.array(tenBlock['BHP_BY_FOC'])) # from annexC(연료소모량 계산)
            #M &= self.Cha.Chauvent(np.array(tenBlock['REL_WIND_SPEED'])) # from anemometer            
            M &= self.Cha_A.Chauvent_Angle(np.array(tenBlock['REL_WIND_DIR'])) # from anemometer
            #M &= self.Cha_A.Chauvent_Angle(np.array(tenBlock['SHIP_HEADING']))
            #M &= self.Cha_A.Chauvent_Angle(np.array(tenBlock['RUDDER_ANGLE']))            
            #M &= self.Cha.Chauvent(np.array(tenBlock['WATER_DEPTH']))
            #M &= self.Cha.Chauvent(np.array(tenBlock['DRAFT_FORE']))
            #M &= self.Cha.Chauvent(np.array(tenBlock['DRAFT_AFT']))
            #M &= self.Cha.Chauvent(np.array(tenBlock['SST'])) # Seawater Temp.
            #M &= self.Cha.Chauvent(np.array(tenBlock['AIR_TEMP']))
            
            # Validation filter
            #M &= self.StdE.StandardError(np.array(tenBlock['SHAFT_REV'], 3.))
            #M &= self.StdE.StandardError(np.array(tenBlock['SPEED_VG'], 0.5))
            #M &= self.StdE_A.StandardError_angle(np.array(tenBlock['RUDDER_ANGLE'], 1.))
            

            i += 1
            if M.all():     
                resultBlock = pd.concat([resultBlock, tenBlock]) 
            else:
                #pass
                #print('filtered')
                filteredBlock = pd.concat([filteredBlock, tenBlock])

            t += tenM  # 다음 10분 block 수행
            tenBlock = values[(t <= values.TIME_STAMP) & (values.TIME_STAMP < t + tenM)] # 첫 10분 block과 마찮가지로 조건인덱싱으로 추출


        #resultDf['VG_Chav'] = vg_chav
        #resultDf['LW_Chav'] = lw_chav

        print('{} vLen : {}, result len : {}'.format(i, len(values), len(resultBlock)))
        resultBlock.to_csv("resultBlock.csv") # filter를 거친 data csv로 저장
        filteredBlock.to_csv("filteredBlock.csv") # filter로 걸러진 data 저장
        #resultDF.to_csv("Data_mask") # true, false

        return resultBlock # 다른 py 파일에서 결과값 쓸수있도록 반환


# Filter 정의
class DataFilter:
    """
    Numpy base로 수정해야됨
    """
    def Chauvent(self, values):  #DataFrame이 아닌, seriese value로 받아서 filtering
        #values = np.array(values) # 불러온 data를 dataframe 형태에서, array 형태로 변경
        mean = values.mean()  #평균        
        delta =  abs(values - mean) #delta = abs(values - mean)
        N = len(values)  #변수 갯수
        sigma = np.sqrt((1./N)*np.sum(delta**2))  #standard error of the mean        

        if sigma !=0:
            dii = delta/(sigma*np.sqrt(2))      
            Pdi = np.vectorize(lambda x : math.erfc(x)*N >= 0.5)#math.erfc(x)*N < 0.5 ? false : true
            retrv = Pdi(dii)   
        else:
            retrv = np.ones(N, dtype=bool)

        return retrv
        
    def Chauvent_Angle(self, values):  #DataFrame이 아닌, seriese value로 받아서 filtering
        #values = np.array(values) # 불러온 data를 dataframe 형태에서, array 형태로 변경
        mean_sin = np.sin((values/(180/np.pi)).mean())  # sin 평균 
        mean_cos = np.cos((values/(180/np.pi)).mean())  # cos 평균
        mean = math.atan2(mean_cos, mean_sin)
        mean = mean * (180/np.pi) 

        delta =  abs(values - mean) #delta = abs(values - mean)
        N = len(values)  #변수 갯수
        sigma = np.sqrt((1./N)*np.sum(delta**2))  #standard error of the mean        

        if sigma !=0:
            dii = delta/(sigma*np.sqrt(2))      
            Pdi = np.vectorize(lambda x : math.erfc(x)*N >= 0.5)#math.erfc(x)*N < 0.5 ? false : true
            retrv = Pdi(dii)   
        else:
            retrv = np.ones(N, dtype=bool)    

        return retrv


    def StandardError(self, values, StdE): # 'SHAFT_REV'(RPM), 'SPEED_VG', 'SPEED_LW'
        mean = values.mean()  #평균     
        delta = abs(values - mean)  #delta = abs(values - mean), 각각 행을 계산(lambda이용)
        N = len(df)  #변수 갯수
        sigma = np.sqrt((1./N)*np.sum(delta**2))
        #print(mean, delta, N,sigma)

        stE = np.vectorize(lambda x :x < StdE) 
        retrv = stE(sigma)

        if retrv.all():
            retrv = np.ones(N, dtype=bool)
        else:
            pass

        return retrv 
        

    def StandardError_angle(self, values, StdE): # 'RUDDER_ANGLE'
        N = len(df)  #변수 갯수
        mean_sin = np.sum(np.sin(values/(180/np.pi)))/N # sin 평균 
        mean_cos = np.sum(np.cos(values/(180/np.pi)))/N # cos 평균
        mean = (np.arctan2(mean_sin,mean_cos))*(180/np.pi)
        
        delta = np.mod(abs(values - mean),360)
        sigma = np.sqrt((1./N)*np.sum(delta**2))
        print(mean_sin,mean_cos,mean, N,sigma)

        stE = np.vectorize(lambda x :x < StdE) 
        retrv = stE(sigma)

        if retrv.all():
            retrv = np.ones(N, dtype=bool)
        else:
            pass

        return retrv 
    



# 다른 파일에서 본 파일을 부르면 실행 되지 않고, 행당 파일에서만 실행 되는 부분
if __name__ == '__main__':
    # data 준비
    # 1. data 불러오기
    datain = di.Init('3FFB8','2017-02-22 00:00','2017-02-22 23:00') # server에서 불러오기(하루 data임. 소장님께서 비교 항차기간 결정후 수정 예정)
    df = datain.queryAll(isShuffle=False, isPD=True, meanTime = 0)
    df.to_csv("original_Df.csv") 

    # 2. data 정렬(혹시, sorting이 안되어 있을수도 있으니...시간순으로 오름차순 sorting)
    df = df.sort_values(["TIME_STAMP"], ascending = [True]) # ascending = [True] : 오름차순 / [False] : 내림차순

    #실행 구문
    f = DataFilter() #인스턴스 정의, DataFilter class실행
    datablock = Datablock(f,f,f,f) # Datablock class init 실행 (값->DataFilter의 함수 받음) 
    df2 = datablock.run(df) # datablock의 run def 실행


end = pd.to_datetime(strftime("%y%m%d-%H%M%S"))

print(end-start)
