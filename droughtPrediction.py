
import pandas as pd
import numpy as np
import statistics
import csv
from sklearn.naive_bayes import GaussianNB
from sklearn import metrics
import matplotlib.pyplot as plt
from sklearn.datasets import load_wine
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import StandardScaler
from sklearn.decomposition import PCA

#return a array holding two dictionary.
#index 0 - training data
#           contains each following month and the each year classification.
#index 1 - test data
#           contains each following month and the each year classification.
def getClassifiedData(cityName):
    clean_data_set = pd.read_csv('cleanData/clean_'+cityName+'.csv')

    #print(targetFile)

    training = {
        "January":  {}, 
        "February": {}, 
        "March":    {}, 
        "April":    {}, 
        "May":      {}, 
        "June":     {}, 
        "July":     {},
        "August":   {},
        "September":{}, 
        "October":  {}, 
        "November": {},
        "December": {} 
    }
    test = {
        "January":  {}, 
        "February": {}, 
        "March":    {}, 
        "April":    {}, 
        "May":      {}, 
        "June":     {}, 
        "July":     {},
        "August":   {},
        "September":{}, 
        "October":  {}, 
        "November": {},
        "December": {} 
    }
    for i, row in clean_data_set.iterrows():
        date = row[0]
        date = str(date).split('/')
        month = date[0]
        month = getMonth(int(month))
        year = date[2]
        avg_prcp = row[1]
        spi = row[2]
        numClass = getSPIClassification(spi)
        classification = row[3]
        data = [avg_prcp, spi, numClass, classification]
        if year == '2018':
            innerDict = test[month]
            date = month + ' ' + year
            #print(date, numClass)
            if numClass not in test[month]:
                '''training[year] = {}
                tempMonthDict = training[year] 
                tempMonthDict[month] = data'''
                tempDict = {0:[], 1:[], 2:[], 3:[], 4:[], 5:[], 6:[]}
                arr1 =  [avg_prcp]
                tempDict[numClass].append(arr1)
                test[month] = tempDict
            else:
                '''tempMonthDict = training[year] 
                tempMonthDict[month] = data'''
                matrix1 = innerDict[numClass]
                #print(innerDict[numClass])
                matrix1.append([avg_prcp])
                innerDict[numClass] = matrix1
                #trainingMonth[month] = innerDict
                #print(matrix1) 
        else:
            innerDict = training[month]
            date = month + ' ' + year
            #print(date, numClass)
            if numClass not in training[month]:
                '''training[year] = {}
                tempMonthDict = training[year] 
                tempMonthDict[month] = data'''
                tempDict = {0:[], 1:[], 2:[], 3:[], 4:[], 5:[], 6:[]}
                arr1 =  [avg_prcp]
                tempDict[numClass].append(arr1)
                training[month] = tempDict
            else:
                '''tempMonthDict = training[year] 
                tempMonthDict[month] = data'''
                matrix1 = innerDict[numClass]
                #print(innerDict[numClass])
                matrix1.append([avg_prcp])
                innerDict[numClass] = matrix1
                #trainingMonth[month] = innerDict
                #print(matrix1) 

    training_and_test_data = [training,test]
    #print(training)
    #print(test)
    return training_and_test_data

def getMonth(month):
    month = int(month)
    months = {
        1:"January", 
        2:"February",
        3:"March",
        4:"April",
        5:"May",
        6:"June",
        7:"July",
        8:"August",
        9:"September", 
        10:"October", 
        11:"November",
        12:"December"
    }
    #print(months[month])
    return months[month]

def getSPIClassification(SPI):
    if SPI <= -2:
        return 0 #'Extremely dry'
    elif SPI >= -1.99 and SPI <= -1.5:
        return 1 #'Severely dry'
    elif SPI >= -1.49 and SPI <= -1.0:
        return 2 #'Moderately dry'
    elif SPI >= -.99 and SPI <= .99:
        return 3 #'Near normal'
    elif SPI >= 1.0 and SPI <= 1.49:
        return 4 #'Moderately wet'
    elif SPI >= 1.5 and SPI <= 1.99:
        return 5 #'Very wet'
    else: 
        return 6 #'Extermely wet'

def getSPI(SPI):
    if SPI == 0:
        return 'Extremely dry'
    elif SPI == 1:
        return 'Severely dry'
    elif SPI == 2:
        return 'Moderately dry'
    elif SPI == 3:
        return 'Near normal'
    elif SPI == 4:
        return 'Moderately wet'
    elif SPI == 5:
        return 'Very wet'
    else: 
        return 'Extermely wet'

def seperateByClass(data):
    trainingMonth = {
            "January":  {}, 
            "February": {}, 
            "March":    {}, 
            "April":    {}, 
            "May":      {}, 
            "June":     {}, 
            "July":     {},
            "August":   {},
            "September":{}, 
            "October":  {}, 
            "November": {},
            "December": {} 
        }
    #seperate all training data into months & classification.
    print(data)
    for year in data:
        print(year)
        monthDict = data[year]
        for month in monthDict:
            data = monthDict[month]
            date = month + ' ' + str(year)
            avg_prcp = data[0]
            spi = data[1]
            classSPI = data[2]
            
            innerDict = trainingMonth[month]
            if classSPI not in innerDict:
                tempDict = {}
                tempDict[classSPI] = [[date, avg_prcp, spi]]
                trainingMonth[month] = tempDict
            else:
                matrix1 = innerDict[classSPI]
                matrix1.append([date, avg_prcp, spi]) 
    return trainingMonth

#Breaks down the Training & Test data set in order to 
#use GAUSSIAN NAIVE BAYER.
def getPredication(training, test):
    #print(training)
    #getting
    predictedResults = []
    expectedResults = []
    for month in training:
        if month == 'November':
            break
        X = [] #holds all individual attribute results of a single classification index in Y.
        Y = [] #hold all classification results.
        classes1 = training[month]
        classes2 = test[month]
        predictData = []
        expected = -1
        for SPIClass in classes1:
            classNum = SPIClass
            #print(month,classNum)
            SPIClass1 = classes1[SPIClass]
            SPIClass2 = classes2[SPIClass]
            n_samples = 0 #if no samples
            n_feature = 0 #if no features
            if len(SPIClass1) != 0: #adds the classificaion if 
                n_samples = len(SPIClass1)
                n_feature = 2 #just two features: avg_prcp & spi
                shape = np.array(SPIClass1)
                #each classification stores all its attributes results
                #This a way to break down and set up for Sklearn GuassianNB.
                for trainingAttributes in shape:
                    X.append(trainingAttributes)
                    Y.append(int(classNum))
                
                
            if len(SPIClass2) != 0:
                predictData = SPIClass2
                expected = int(classNum)
            #break
        
                
            #shape = [n_samples,n_feature]

            #X.append(shape)
            #Y.append(int(classNum))

        #GAUSSIAN NAIVE BAYER
        clf = GaussianNB()
        X = np.array(X)
        Y = np.array(Y)
        #print(month, 'prediction result', result,'Classification:',getSPI(result[0]))
        #print('X:', X, len(X))
        #print('Y:',Y, len(Y))
        #print('predictData',predictData)
        clf.fit(X,Y)
        result = clf.predict(predictData)
        #print(month,X,Y)
        #print('\tprediction result', result)
        #print('\tClassification:',getSPI(result[0]))
        print(month, 'Classification prediction:',getSPI(result[0]))
        print()
        predictedResults.append(result[0])
        expectedResults.append(expected)
        pD = np.array(predictData)
        r = np.array(result)
        #visualizeData(X,pD,Y, r,clf)
    #print(expectedResults)
    #print(predictedResults)
    print('classification_ report:', metrics.classification_report(expectedResults,predictedResults))
    print('confusion_matrix:',metrics.confusion_matrix(expectedResults,predictedResults))
    print('accuaracy ',metrics.accuracy_score(expectedResults,predictedResults))
    #plt.show()
                
#DOESN'T WORK        
def visualizeData(X_train, X_test, y_train, y_test,clf):

    xlim = (-1, 8)
    ylim = (-1, 5)
    xx, yy = np.meshgrid(np.linspace(xlim[0], xlim[1], 71),
                        np.linspace(ylim[0], ylim[1], 81))
    Z = clf.predict_proba(np.c_[xx.ravel(), yy.ravel()])
    Z = Z[:, 1].reshape(xx.shape)

    # Plot the results
    fig = plt.figure(figsize=(5, 3.75))
    ax = fig.add_subplot(111)
    #ax.scatter(X_train[:, 0], X_train[:, 1], c=y_train, cmap=plt.cm.binary, zorder=2)

    ax.contour(xx, yy, Z, [0.5], colors='k')

    ax.set_xlim(xlim)
    ax.set_ylim(ylim)

    ax.set_xlabel('$x$classification')
    ax.set_ylabel('$y$features')
    ax.set_title('')

    

            

def main():

    city_data = getClassifiedData('MONTEREY, CA US')
    print('------------------MONTEREY, CA US----------------------')
    getPredication(city_data[0], city_data[1])
    print('------------------MONTEREY, CA US----------------------')


    city_data = getClassifiedData('REDDING, CA US')
    print('------------------REDDING, CA US----------------------')
    getPredication(city_data[0], city_data[1])
    print('------------------REDDING, CA US----------------------')

    city_data = getClassifiedData('RIVERSIDE, CA US')
    print('------------------RIVERSIDE, CA US----------------------')
    getPredication(city_data[0], city_data[1])
    print('------------------RIVERSIDE, CA US----------------------')

    city_data = getClassifiedData('VISALIA, CA US')
    print('------------------VISALIA, CA US----------------------')
    getPredication(city_data[0], city_data[1])
    print('------------------VISALIA, CA US----------------------')


main()