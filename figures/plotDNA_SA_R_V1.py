# -*- coding: utf-8 -*-
"""
Created on Fri Jun  9 10:40:24 2023

@author: frede
"""

import os
import seaborn as sns
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

path = os.getcwd()
print(path)

os.chdir(path+"/StringIndexingGapResults")
path = os.getcwd()
print(path)
df = pd.read_csv('SA_R_V1_0.csv')

for i in range(1,10):
    df2 = pd.read_csv('SA_R_V1_'+str(i)+'.csv')
    df2 = df2[df2['data'].str.startswith('DNA_')]
    df = pd.concat([df,df2])
    
df = df[df['data'].str.startswith('DNA_')]

dataLength = df['data'].str[4:]
df['dataLength'] = dataLength
df = df.groupby('dataLength').sum()
df = df.replace(0,1)
df = df.sort_values('dataLength')


"""
Plotting time (y) vs size (x), on top single pattern
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='topSingle')
#plt.show()
plt.savefig("DNA_SA_R_V1_TopSingle.png")

"""
Plotting time (y) vs size (x), on top fixed gap
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='topFixed')
#plt.show()
plt.savefig("DNA_SA_R_V1_TopFixed.png")


"""
Plotting time (y) vs size (x), on top variable gap
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='topVariable')
#plt.show()
plt.savefig("DNA_SA_R_V1_TopVariable.png")

"""
Plotting time (y) vs size (x), on mid single pattern
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='midSingle')
#plt.show()
plt.savefig("DNA_SA_R_V1_MidSingle.png")


"""
Plotting time (y) vs size (x), on mid fixed gap
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='midFixed')
#plt.show()
plt.savefig("DNA_SA_R_V1_MidFixed.png")


"""
Plotting time (y) vs size (x), on mid variable gap
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='midVariable')
#plt.show()
plt.savefig("DNA_SA_R_V1_MidVariable.png")


"""
Plotting time (y) vs size (x), on bot single pattern
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='bottomSingle')
#plt.show()
plt.savefig("DNA_SA_R_V1_BotSingle.png")

"""
Plotting time (y) vs size (x), on bot fixed gap
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='bottomFixed')
#plt.show()
plt.savefig("DNA_SA_R_V1_BotFixed.png")


"""
Plotting time (y) vs size (x), on bot variable gap
"""

sns.set()
plt.figure()
plt.yscale('log')
ax = sns.lineplot(data = df, x='dataLength', y='bottomVariable')
#plt.show()
plt.savefig("DNA_SA_R_V1_BotVariable.png")
    

#sns.set()
#plt.figure()

#ax = sns.lineplot(data = df, hue='set', x='size', y='time', palette=['blue', 'red', 'yellow', 'grey'])
#plt.show()


