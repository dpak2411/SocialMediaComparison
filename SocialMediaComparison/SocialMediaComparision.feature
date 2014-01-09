Feature: SocialMediaComparision
Find out what people are tweeting about your business or business venture 

Background:
  Given alteryx running at" http://gallery.alteryx.com/"
  And I am logged in using "deepak.manoharan@accionlabs.com" and "P@ssw0rd"



Scenario Outline:  the app with same keyword with different locations
When I run the app <app> that searches the twitter handle "<handle>"
And I pass the first location details "<locationname1>","<Address1>","<City1>","<State1>","<ZipCode1>"
And I also pass the second location details "<locationname2>","<Address2>","<City2>","<State2>","<ZipCode2>"
And I choose the radius and the size of area to study "<SA>" in miles
Then I see the output has the report "<output>"

Examples: 
| app                                       | handle  | locationname1 | Address1                    | City1      | State1 | ZipCode1 | locationname2 | Address2                  | City2  | State2 | ZipCode2 | SA | output|
| "Marketing - Social Media Comparison App" | alteryx | Test1         | "1121 Boyce Road ,Ste 1400" | Pittsburgh | PA     | "15102"    | Test2         | "230 Commerce, Suite 250" | Irvine | CA     | "92602"    | 10 |Social Media Location Report   | 

#Scenario Outline:  the app with same keyword with different locations
#When I run the app <app> that searches the twitter handle "<handle>"
#And I pass the first location details "<locationname1>","<Address1>","<City1>","<State1>","<ZipCode1>"
#And I also pass the second location details "<locationname2>","<Address2>","<City2>","<State2>","<ZipCode2>"
#And I choose the radius and the size of area to study "<SA>" in miles
#Then I see the SentimentScore more than <SentimentScore> for screenname "<screenname>"
#
#Examples: 
#| app                                       | handle  | locationname1 | Address1                    | City1      | State1 | ZipCode1 | locationname2 | Address2                  | City2  | State2 | ZipCode2 | SA | SentimentScore | screenname |
#| "Marketing - Social Media Comparison App" | alteryx | Test1         | "1121 Boyce Road ,Ste 1400" | Pittsburgh | PA     | "15102"    | Test2         | "230 Commerce, Suite 250" | Irvine | CA     | "92602"    | 10 | 5              | alteryx    | 
#





