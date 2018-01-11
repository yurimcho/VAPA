"""
Filtering된 data를 받아와서 성능해석하는 파일임.
"""

import pymssql
import numpy as np
import pandas as pd
import math
import DataFilter as DF # DataFilter.py의 모듈을 가져옴


class CalPerformance:
           
    def wind_correction(self): 
    # corrected power를 셀프로 하나더 만들자. 처음 파워와 비교
    # true wind speed, direction 계산 
        pass

    def calculte_PV(self):     
        pass
    
    def speedpowerTable(self): 
    #동일한 draft에서 읽어와야되고, base line이 필요.(reference line 생성)
    # draft 보정(가상선 으로하고 있음. - 19030에 나와있음 page5) 
        pass

    def expactedspeed(self): 
    #power에 따른 속도 PV는 각각의 set에 대해 모두
        pass

    def referencefilter(self): 
    # PI구하기 전에 filter 한번더
        pass

    def calculation_pi(self): 
    #기간평균, 속도 저감
        pass

    def calculation_ppv(self): 
    #기간평균, power 향상
        pass

    def calculate_bhp(self):  
    #AnnexB, AnnexC X particular안에 있음 Annex D(pa) - SFOC와 power 순환 구조?
        pass

    def claculate_power(self):
        pass

    def correction_wind(self):
        pass


if __name__ == '__main__':
    # data 준비
    # 1. data 불러오기
    #datain = di.Init('3FFB8','2017-02-22','2017-02-25') # server에서 불러오기(하루 data임. 소장님께서 비교 항차기간 결정후 수정 예정)
    df = datain.queryAll(isShuffle=False, isPD=True, meanTime = 0)
    df = Filter.df_AftFilter # Filter에서 가져오는 방법
    '''
    df = pd.read_csv('sample.csv') # csv로 부르기
    del df['Unnamed: 0']
    df['TIME_STAMP'] = pd.to_datetime( df['TIME_STAMP'] )
    '''
    # 2. data 정렬(혹시, sorting이 안되어 있을수도 있으니...시간순으로 오름차순 sorting)
    df = df.sort_values(["TIME_STAMP"], ascending = [True]) # ascending = [True] : 오름차순 / [False] : 내림차순
    #print(df.head(5)) # 정렬확인
    
    f = DF.DataFilter() #인스턴스 정의, DataFilter class실행
    datablock = DF.Datablock(f,f,f,f) # Datablock class init 실행 (값->DataFilter의 함수 받음) 
    df2 = datablock.run(df) # datablock의 run def 실행
    
    A = CalPerformance()  #인스턴스 정의
    #A_df = A.wind_correction(0)
    #print(A_df)
