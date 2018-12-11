
import pandas as pd
import statistics
import csv

#Writes cleaned out data to a csv file.
def writeToCSV(city, data):
    with open('clean_'+city+'.csv','w', newline='') as csvfile:
        filewriter = csv.writer(csvfile, delimiter=',')
        filewriter.writerow(['Date', 'Average PRCP', 'SPI', 'SPI Classification'])
        for date in data:
            csvRow = [date]
            tempArrInfo = data[date]
            csvRow.append(tempArrInfo[0])
            csvRow.append(tempArrInfo[1])
            csvRow.append(tempArrInfo[2])
            filewriter.writerow(csvRow)

#Takes in city name and returns a
#dictionary holding every years data.
#each year is going to contain 12 months with
#each month having:
#   avg precpitation, SPI, & SPI classification.
def getCleanOutData(cityName):
    avg_data_set = pd.read_csv('preprocData1/'+cityName+'.csv')
    spi_data_set = pd.read_csv('preprocData1/'+cityName+'_SPI_M_01.csv')

    #print(targetFile)

    history = {}
    for i, row in avg_data_set.iterrows():
        date = row[0]
        date = str(date).split('-')
        #print(date)
        date = getDateFormat(date)
        #print(date)
        avg_prcp = row[1]
        history[date] = [avg_prcp]
    #print(history)

    for i, row in spi_data_set.iterrows():
        date = str(row[0])
        spi_part1 = row[1]
        spi_part2 = row[2]
        spi_part1 = str(spi_part1).replace('"','')
        spi_part2 = str(spi_part2).replace('"','')
        spi = ''
        if(spi_part1 == 'nan'):
            spi = '0' +'.'+spi_part2
        elif(spi_part2 == 'nan'):
            spi = spi_part1 +'.'+'0'
        else:
            spi = spi_part1 +'.'+spi_part2
        spi = float(spi)
        #print(spi_part1, spi_part2)
        #print(spi)
        #print(date)
        tempArr = history[date]
        #print(tempArr)
        tempArr.append(spi)
        spiClass = getSPIClassification(spi)
        tempArr.append(spiClass)
    #print(history)
    return history

#created it to help extract into the correct date for the history dictionary
#in getCleanOutData(cityName) function.
def getDateFormat(arr):
    dict1 = {
        'Jan':'1/1',
        'Feb':'2/1',
        'Mar':'3/1',
        'Apr':'4/1',
        'May':'5/1',
        'Jun':'6/1',
        'Jul':'7/1',
        'Aug':'8/1',
        'Sep':'9/1',
        'Oct':'10/1',
        'Nov':'11/1',
        'Dec':'12/1'
    }
    year = int(arr[1])
    if(year >= 95):
        year = '19'+str(year)
    elif(year >= 10): 
        year = '20'+str(year)
    else:
        year = '200'+str(year)
    date = dict1[arr[0]] +'/' + year
    return date

def getSPIClassification(SPI):
    if SPI <= -2:
        return 'Extremely dry'
    elif SPI >= -1.99 and SPI <= -1.5:
        return 'Severely dry'
    elif SPI >= -1.49 and SPI <= -1.0:
        return 'Moderately dry'
    elif SPI >= -.99 and SPI <= .99:
        return 'Near normal'
    elif SPI >= 1.0 and SPI <= 1.49:
        return 'Moderately wet'
    elif SPI >= 1.5 and SPI <= 1.99:
        return 'Very wet'
    else: 
        return 'Extermely wet'

monterey_dataset = getCleanOutData("MONTEREY, CA US")
writeToCSV("MONTEREY, CA US",monterey_dataset)

redding_dataset = getCleanOutData("REDDING MUNICIPAL AIRPORT, CA US")
writeToCSV("REDDING, CA US",redding_dataset)

riverside_dataset = getCleanOutData("RIVERSIDE FIRE STATION 3, CA US")
writeToCSV("RIVERSIDE, CA US",riverside_dataset)

ukiah_dataset = getCleanOutData("UKIAH, CA US")
writeToCSV("UKIAH, CA US",ukiah_dataset)

visalia_dataset = getCleanOutData("VISALIA, CA US")
writeToCSV("VISALIA, CA US",visalia_dataset)





