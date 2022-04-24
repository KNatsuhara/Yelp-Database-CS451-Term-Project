
import json
import psycopg2

def cleanStr4SQL(s):
    return s.replace("'","`").replace("\n"," ")

def recursiveAttributesParser(d):

    attributes = []
    d_items = d.items()
    for item in d_items:
        if type(item[1]) == type(dict()):
            attributes = attributes + recursiveAttributesParser(item[1])
        else:
            attributes.append(item)
    return attributes

def hoursParser(t):
    hours = []
    t_items = t.items()
    for item in t_items:
        hours.append((item[0], item[1].split('-')))
    return hours
    
def parseBusinessData():

    #read the JSON file
    # We assume that the Yelp data files are available in the current directory. If not, you should specify the path when you "open" the function. 
    with open('.//yelp_business.JSON','r') as f:  

        # connect to yelpdp on postgres server using psycopg2
        try:
            #TODO: update the database name, username, and password
            conn = psycopg2.connect("dbname='fancyfriendsdb' user='postgres' host='localhost' password='YOUR PASS HERE'")
        except:
            print("Unable to connect to the database!")
        cur = conn.cursor()

        line = f.readline()
        count_line = 0

        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            business_id = cleanStr4SQL(data['business_id'])

            # parse and generate insert statement
            try:
                cur.execute("INSERT INTO Business (business_id, name, address, state, city, zip_code, latitude, longitude, stars, num_checkins, num_tips, is_open)"
                    + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
                    (
                        business_id,
                        cleanStr4SQL(data["name"]),
                        cleanStr4SQL(data["address"]),
                        cleanStr4SQL(data["state"]),
                        cleanStr4SQL(data["city"]),
                        cleanStr4SQL(data["postal_code"]),
                        str(data["latitude"]),
                        str(data["longitude"]),
                        data["stars"],
                        "0",
                        "0",
                        str([False,True][data["is_open"]]))
                    )
            except Exception as e:
                print("Insert to Business TABLE failed!", e)
            conn.commit()

            # process business categories
            categories = data["categories"].split(', ')     # categories = list ['name'] ex: ['Sushi Bars', 'Restaurants']

            for category in categories:
                try:
                    cur.execute("INSERT INTO Category (name, business_id)"
                        + " VALUES (%s, %s)",
                        (
                            category,
                            business_id
                        ))
                except Exception as e:
                    print("Insert to Category TABLE failed!", e)
                conn.commit()
            
            # process business attributes
            # make sure to **recursively** parse all attributes at all nesting levels. You should not assume a particular nesting level. 
            attributes = recursiveAttributesParser(data["attributes"])  # attribues = list [(name, value)] ex: [('latenight', 'False')]

            for attribute in attributes:
                try:
                    cur.execute("INSERT INTO Attribute (name, value, business_id)"
                        + " VALUES (%s, %s, %s)",
                        (
                            attribute[0],
                            attribute[1],
                            business_id
                        ))
                except Exception as e:
                    print("Insert to Attribute TABLE failed!", e)
                conn.commit()

            # process business hours
            hours = hoursParser(data["hours"])  # hours = list [(day, [open, close])] ex: [('Tuesday', ['10:0', '18:0'])]

            for hour in hours:
                try:
                    cur.execute("INSERT INTO Hours (business_id, day, open_time, closing_time)"
                        + " VALUES (%s, %s, %s, %s)",
                        (
                            business_id,
                            hour[0],
                            hour[1][0],
                            hour[1][1]
                        ))
                except Exception as e:
                    print("Insert to Attribute TABLE failed!", e)
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(str(count_line) + " businesses parsed")
    f.close()

def parseUserData():

    # read the JSON file
    # We assume that the Yelp data files are available in the current directory. If not, you should specify the path when you "open" the function. 
    with open('.//yelp_user.JSON','r') as f:  

        # connect to yelpdp on postgres server using psycopg2
        try:
            #TODO: update the database name, username, and password
            conn = psycopg2.connect("dbname='fancyfriendsdb' user='postgres' host='localhost' password='YOUR PASS HERE'")
        except:
            print("Unable to connect to the database!")
        cur = conn.cursor()

        line = f.readline()
        count_line = 0

        all_friends = []    # we have to add the friends after we add the users. ex: the first user is added, none of their friends user_id's are in yet so we cant reference the foreign key
        index = 0

        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            user_id = cleanStr4SQL(data['user_id'])

            full_name = cleanStr4SQL(data["name"])
            first_name = ""
            last_name = ""
            try:
                if full_name.split()[0]:
                    first_name = full_name.split()[0]
                if full_name.split()[1]:
                    last_name = full_name.split()[1]
            except IndexError:
                index += 1

            # parse and generate insert statement
            try:
                cur.execute("INSERT INTO Users (user_id, first_name, last_name, total_tip_count, total_tip_likes, average_stars, cool, funny, useful, date_joined, number_of_fans, longitude, latitude, count_of_votes)"
                    + " VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)",
                    (
                        user_id,
                        first_name,
                        last_name,
                        data["tipcount"],
                        "0",
                        data["average_stars"],
                        data["cool"],
                        data["funny"],
                        data["useful"],
                        cleanStr4SQL(data["yelping_since"]),
                        data["fans"],
                        "0",
                        "0",
                        "0"
                    ))
            except Exception as e:
                print("Insert to Users TABLE failed!", e)
            conn.commit()

            # process friends
            all_friends.append((user_id,data["friends"]))   # all friends = list [(user_id, [friends])] ex: [('sdakfjfhwr', ['adskjnf8ysafd','sadjfhas9d8'])]

            line = f.readline()
            count_line += 1

        # insert to friends table AFTER we have all the user_id's in so we can reference the foreign keys
        for user_friends in all_friends:    # user_friends = (user_id, [friends])
            for friend in user_friends[1]:  
                try:
                    cur.execute("INSERT INTO Friends (user_id, user_id2)"
                        + " VALUES (%s, %s)",
                        (
                            user_friends[0],
                            friend
                        ))
                except Exception as e:
                    print("Insert to Friends TABLE failed!", e)
                conn.commit()

        cur.close()
        conn.close()

    print(str(count_line) + " user datas parsed")
    print(str(index) + " index errors")
    f.close()

def checkinHelper(timestamp):
    timestamp = timestamp.replace('-', ' ')
    list = timestamp.split()
    year = list[0]
    month = list[1]
    day = list[2]
    time = list[3]
    return year, month, day, time

def parseCheckinData():

    # read the JSON file
    # We assume that the Yelp data files are available in the current directory. If not, you should specify the path when you "open" the function. 
    with open('.//yelp_checkin.JSON','r') as f:  

        # connect to yelpdp on postgres server using psycopg2
        try:
            #TODO: update the database name, username, and password
            conn = psycopg2.connect("dbname='fancyfriendsdb' user='postgres' host='localhost' password='YOUR PASS HERE'")
        except:
            print("Unable to connect to the database!")
        cur = conn.cursor()

        line = f.readline()
        count_line = 0 

        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            # parse and generate insert statement
            checkins = data["date"].split(',') 
            business_id = cleanStr4SQL(data['business_id'])

            for date in checkins:
                checkin = checkinHelper(date)   # checkin = list [(year, month, day, time)] ex: [('2010', '05', '02', '23:57:32')]
                try:
                    cur.execute("INSERT INTO Check_in (business_id, check_in_time, day, month, year)"
                        + " VALUES (%s, %s, %s, %s, %s)",
                        (
                            business_id,
                            checkin[3],
                            checkin[2],
                            checkin[1],
                            checkin[0]
                        ))
                except Exception as e:
                    print("Insert to Check_in TABLE failed!", e)
                conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(str(count_line) + " checkins parsed")
    f.close()

def parseTipData():

    #read the JSON file
    # We assume that the Yelp data files are available in the current directory. If not, you should specify the path when you "open" the function. 
    with open('.//yelp_tip.JSON','r') as f:  

        # connect to yelpdp on postgres server using psycopg2
        try:
            #TODO: update the database name, username, and password
            conn = psycopg2.connect("dbname='fancyfriendsdb' user='postgres' host='localhost' password='YOUR PASS HERE'")
        except:
            print("Unable to connect to the database!")
        cur = conn.cursor()

        line = f.readline()
        count_line = 0

        #read each JSON abject and extract data
        while line:
            data = json.loads(line)

            try:
                cur.execute("INSERT INTO Tip (business_id, user_id, tip_time, tip_text, likes)"
                    + " VALUES (%s, %s, %s, %s, %s)",
                    (
                        cleanStr4SQL(data['business_id']),
                        cleanStr4SQL(data["user_id"]),
                        cleanStr4SQL(data["date"]),
                        cleanStr4SQL(data["text"]),
                        data["likes"]
                    ))
            except Exception as e:
                print("Insert to Tip TABLE failed!", e)
            conn.commit()

            line = f.readline()
            count_line +=1

        cur.close()
        conn.close()

    print(str(count_line) + " tips parsed")
    f.close()

if __name__ == "__main__":
    parseBusinessData()
    parseUserData()
    parseCheckinData()
    parseTipData()