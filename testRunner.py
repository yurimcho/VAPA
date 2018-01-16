import sys
import os
from data_process import datain as di
from data_process import dataout as do

datain = di.Init('3FFB8','2017-02-20','2017-10-22')
# shipDatas = datain.queryAll(isShuffle=False, isPD=True, meanTime = 0)
# print(shipDatas)
shipParticular = datain.QueryShipParticualr()
# print(shipParticular)

print(shipParticular)

dataout = do.Init(shipParticular)
# dataout.to_print(['SPEED_VG'])
dataout.to_csv()

print("앙")