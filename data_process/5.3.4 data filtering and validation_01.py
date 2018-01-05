""" ISO19030 
    5.3 Data Preparation 
    5.3.4 data filtering and validation
     1) make 10minute data blocks
     2) Chauvent criteriton filter (Annex I)
     3) Standard Error of mean validation (Annex J)

--------------------------
주) 2018.01.05
1. 아래 코드는 10분 block에 대한 4부분의 filter중 'Chauvenet's criterion'(not measured as angles) 부분임.
   - filter : Chauvenet(not as angles & as angles) and Validation_StdError(not as angles & as angles)
2. 지난 회의 때, filter 부분을 완성 했었으나, 
   10분 block으로 만드는 부분을 완전히 잘못 생각 한 것을 어제 발견하여 현재 수정하여 테스트 중임.
   유효하지 않은 data의 10분 block내의 data가 모두 없어져야하는데.... 한 값만 filtering 되고있음. 수정필요.
3. Chavenet's criterion은 excel로 만들어 검토하였음. 
4. class, def, 조건문 등에 대해 4가지 filter를 모두 검토 가능하도록 수정 중.
5. Numpy기반으로 추후 수정 예정
---------------------------
"""


import numpy as np
import pandas as pd
from pandas import Series
import sys
import os
import math
sys.path.append(os.path.join(sys.path[0],'data_process'))
sys.path.append(os.path.join(sys.path[0],'data_analysis'))
sys.path.append(os.path.join(sys.path[0],'helper'))
import datain as di
import dataout as do


class Init:    
    def __init__(callSign='3ewb4', beginDate='2017-01-01', endDate='2017-01-10'): 
        self.callSign = callSign
        self.beginDate = beginDate
        self.endDate = endDate



class ChvFilter: #AnnexI - Outlier detection(Chauvenet's criterion)
    def Chauvent(self, values):  #DataFrame이 아닌, seriese value로 받아서 filtering
        mean = values.mean()  #평균        
        delta = values.apply(lambda x : abs(x - mean))  #delta = abs(values - mean), 각각 행을 계산(lambda이용)
        N = len(values)  #변수 갯수
        sigma = math.sqrt((1/N)*sum(delta**2))  #standard error of the mean
        

        if sigma !=0:
            di = delta.apply(lambda x : x/(sigma*math.sqrt(2)))       
            retrv = di.apply(lambda x : math.erfc(x)*N >= 0.5)  #math.erfc(x)*N < 0.5 ? false : true
        else:
            retrv = True        


        return retrv


class Datablock(ChvFilter): # ChvFilter class 내의 def. 사용하도록 class 상속
    def __init__(self, filter):
        self.filter = filter

    def Datablock(self, values):
        sTime = df.iloc[0,0] # 시작시간 가져옴
        t = sTime 
        tenM = pd.Timedelta('10min') # 10분 delta time으로...
        tenBlock = df[df.TIME_STAMP < sTime + tenM] # 조건 인덱싱으로 첫 10분 간격의 데이터 추출

        resultBlock = None # filtering 되지 않은 data 저장 
        #resultfiltering = None

        while len(tenBlock) > 0: #data수가 0보다 많으면 계산 수행
            print(len(tenBlock))
            
            result = ChvFilter.Chauvent() # ChvFilter class 내의 Chauvent def. 실행           

            if result == True:    
                resultBlock += tenBlock  # filter 실행 결과 false값 없을 경우, Chauvent def. 검토한 10분 블럭을 'resultBlock'으로 반환
            else:
                pass
                #resultfiltering += tenBlock

            t += tenM  # 다음 10분 block 수행
            tenBlock = df[(t <= df.TIME_STAMP) & (df.TIME_STAMP < t + tenM)] # 첫 10분 block과 마찮가지로 조건인덱싱으로 추출

        print('total len : {}'.format(len(df)))



if __name__ == '__main__':
    # data 준비
    # 1. data 불러오기
    datain = di.Init('3FFB8','2017-02-22','2017-02-24') # server에서 불러오기(하루 data임. 소장님께서 비교 항차기간 결정후 수정 예정)
    df = datain.queryAll(isShuffle=False, isPD=True, meanTime = 0)

    # 2. data 정렬(혹시, sorting이 안되어 있을수도 있으니...시간순으로 오름차순 sorting)
    df = df.sort_values(["TIME_STAMP"], ascending = [True]) # ascending = [True] : 오름차순 / [False] : 내림차순
    #print(df.head(5)) # 정렬확인

    # 3. data type 확인 
    '''
    Time = df.iloc[0,0]
    m = df.iloc[0,1]
    print(type(sTime))
    print(type(m))
    
    type 변경 필요시... 
     data명 = pd.to.변환type(data명) 형식 이용
       : df['TIME_STAMP'] = pd.to_datetime(df['TIME_STAMP']) 
    '''

    f = Datablock(ChvFilter)  #인스턴스 정의
    initDataN = len(df) # 처음 Data 수
    #print(initDataN)
 
# Chavenet filter -  (열별로 filter 걸기 + data 합치기)
    M = f.Chauvent(df['SPEED_VG'])# SOG(Speed Over Ground)
    M &= f.Chauvent(df['SPEED_LW']) # Longitudinal Water Speed    
    M &= f.Chauvent(df['SHAFT_POWER']) # from shaft power meter[kW]
    M &= f.Chauvent(df['SHAFT_REV']) # from shaft power meter[rpm]
    M &= f.Chauvent(df['SHAFT_TORQUE']) # from shaft power meter[kNm]    
    M &= f.Chauvent(df['BHP_BY_FOC']) # from annexC(연료소모량 계산)
    M &= f.Chauvent(df['REL_WIND_SPEED']) # from anemometer 
    M &= f.Chauvent(df['WATER_DEPTH'])
    M &= f.Chauvent(df['DRAFT_FORE'])
    M &= f.Chauvent(df['DRAFT_AFT'])
    M &= f.Chauvent(df['SST']) # Seawater Temp.
    M &= f.Chauvent(df['AIR_TEMP'])

df_AftFilter = df[M]

AftFilterDataN = len(df_AftFilter)

#print(df_AftFilter)
#df_AftFilter.to_csv("AftFilter.csv") # filtering된 data csv로 저장

print('Data N_init: %d'', '' Data N_After Filter: %d'', ''Filtered Data N: %d' %(initDataN, AftFilterDataN, initDataN-AftFilterDataN))
# 처음 data 수, filtering 후 data 수, filtering된 data 수