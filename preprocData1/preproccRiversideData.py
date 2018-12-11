'''
Each file has Station name, city, preciptation, temperature
Preprocess Data to have:
    There should be file for each station & year.
    Each file will contain each month with 3 attributes.
    The three atributes is:
        -average month preciptattion
        -Standard Preciptation Index(SPI)
        -Palmer Drought Severity Index(PDSI)
'''
import pandas as pd
import statistics
import csv

raw_data_set = pd.read_csv('../rawData/1995-2018Riverside,CA.csv')
#print(targetFile)
arr = raw_data_set

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

cityDict = {}

currentYear = 0
currentMonth = 0
print('Getting csv data...')
#1) iterate throw through raw data csv file
for i, row in raw_data_set.iterrows(): #i=index, j=row
    #2)get raw data for each column data.
    station = row[0]
    location = row[1]
    date = row[2]
    prcp = row[3]
    #3) Get & store a months preciptation.
    #4) If we ecounter a new month. Calulcated the average preciptation,
    #SPI, PDSI, & its class(wet, severe drought, extreme drought, etc). with the all the stored preciptation for that month.
    #5) store calculation for that month.
    #6) repeat step 2 to 4 if you don't encounter a new year.
    #7) When the 12 months averages are recieved for that year.
    #store its own individual file for that year.
    #8) Then repeat step 2 to 7 till it reaches to its last year.
    #print(station, location, date, prcp)

    #print(date)
    date = str(date).split("/")
    #print(date)
    currentMonth = int(date[0])
    currentYear = int(date[2])
    #location = 'MONTEREY, CA US'
    '''monthDict = {
        "January":  [], 
        "February": [], 
        "March":    [], 
        "April":    [], 
        "May":      [], 
        "June":     [], 
        "July":     [],
        "August":   [],
        "September":[], 
        "October":  [], 
        "November": [],
        "December": [] 
    }''' 
    if location not in cityDict:
        yearDict = {}
        yearDict[currentYear] = {
            "January":  [], 
            "February": [], 
            "March":    [], 
            "April":    [], 
            "May":      [], 
            "June":     [], 
            "July":     [],
            "August":   [],
            "September":[], 
            "October":  [], 
            "November": [],
            "December": [] 
        } 
        month = getMonth(currentMonth)
        cityDict[location] = yearDict
        tempYearDict = cityDict[location]
        tempMonthDict = tempYearDict[currentYear]
        if(prcp != 'nan'):
            tempMonthDict[month].append(prcp)
    else:
        yearDict = cityDict[location]
        if currentYear not in yearDict:
            tempYearDict = cityDict[location]
            tempYearDict[currentYear] = {
                "January":  [], 
                "February": [], 
                "March":    [], 
                "April":    [], 
                "May":      [], 
                "June":     [], 
                "July":     [],
                "August":   [],
                "September":[], 
                "October":  [], 
                "November": [],
                "December": [] 
            } 
            month = getMonth(currentMonth)
            tempMonthDict = tempYearDict[currentYear]
            if(prcp != 'nan'):
                tempMonthDict[month].append(prcp)
        else:
            month = getMonth(currentMonth)
            tempYearDict = cityDict[location]
            tempMonthDict = tempYearDict[currentYear]
            if(prcp != 'nan'):
                tempMonthDict[month].append(prcp)

print('Completed getting data.')    
print(len(cityDict))

def getPrecpAverage(data):
    #print(data)
    if(len(data) == 0):
        return 0
    avg = 0
    for i in data:
        avg = avg + i
        #print(i, type(i), avg)
    #print('avg:', avg) 
    avg = avg/len(data)  
    #print('avg:', avg) 
    #print(len(data)) 
    return avg

#for city in cityDict:
city = 'RIVERSIDE FIRE STATION 3, CA US'
#print(city)
tempYearDict = cityDict[city]
with open(city+'.csv','w', newline='') as csvfile:
    filewriter = csv.writer(csvfile, delimiter=',')
    filewriter.writerow([city])
    for year in tempYearDict:
        tempMonthDict = tempYearDict[year]
        print(city, year)
        currentDir = 'C:/Users/moabd/OneDrive/Desktop/CSS490/project'
        
        
        
        for month in tempMonthDict:
            allPrecp = tempMonthDict[month]
            avg = 0.0
            #print(allPrecp)
            avg = getPrecpAverage(allPrecp)
            SPI = 1.0
            if(len(allPrecp) != 0 and avg != 0):
                SPI = statistics.stdev(allPrecp)
            date = str(month)+"/"+str(year)
            strAvg = str(avg)
            if strAvg == 'nan':
                strAvg = "0"
            filewriter.writerow([date,strAvg])
            #print(year , month, avg, SPI)

'''for year in tempYearDict:
        tempMonthDict = tempYearDict[year]
        print(city, year)
        for month in tempMonthDict:
            allPrecp = tempMonthDict[month]
            avg = 0.0
            #print(allPrecp)
            avg = getPrecpAverage(allPrecp)
            SPI = 1.0
            if(len(allPrecp) != 0 and avg != 0):
                SPI = statistics.stdev(allPrecp)
            print(year , month, avg, SPI)'''




    


