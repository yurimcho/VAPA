import sys
import os
sys.path.append(os.path.join(sys.path[0],'data_process'))
sys.path.append(os.path.join(sys.path[0],'data_analysis'))
sys.path.append(os.path.join(sys.path[0],'helper'))
import datain as di
import dataout as do

datain = di.Init('3FFB8','2017-02-20','2017-10-22')
# shipDatas = datain.queryAll(isShuffle=False, isPD=True, meanTime = 0)
# print(shipDatas)
shipParticular = datain.QueryShipParticualr()
# print(shipParticular)

print(shipParticular)

dataout = do.Init(shipParticular)
# dataout.to_print(['SPEED_VG'])
dataout.to_csv()