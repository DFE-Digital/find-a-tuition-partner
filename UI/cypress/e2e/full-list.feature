Feature: Show all quality-assured tuition partners
Scenario: all quality-assured tuition partners page url is '/all-tuition-partners'
    Given a user has arrived on the all quality-assured tuition partners page
    Then the page URL ends with '/all-tuition-partners'

  Scenario: all quality-assured tuition partners page title is 'All Tuition Partners'
    Given a user has arrived on the all quality-assured tuition partners page
    Then the page's title is 'All Tuition Partners'

  Scenario: user clicks service name on all quality-assured tuition partners page
    Given a user has arrived on the all quality-assured tuition partners page
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: link from the service start page
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'Check the full list of quality-assured tuition partners' link
    Then they will see the 'All quality-assured tuition partners' page

  Scenario: full list of quality-assured tuition partners is in alphabetical order by name
    Given a user has arrived on the all quality-assured tuition partners page
    Then the full list of quality-assured tuition partners is in alphabetical order by name

  Scenario: full list of quality-assured tuition partners displays result count
    Given a user has arrived on the all quality-assured tuition partners page
    Then the number of tuition partners displayed matches the displayed count

  Scenario: tuition partner summaries only shows name, website, phone number and email address
    Given a user has arrived on the all quality-assured tuition partners page
    Then the user is only shown the name, website, phone number and email address for each tuition partner

  Scenario: tuition partner summaries' name links to tuition partner details page
    Given a user has arrived on the all quality-assured tuition partners page
    Then the name of each tuition partner links to their details page

  Scenario: tuition partner summaries' website link opens their website in a new tab
    Given a user has arrived on the all quality-assured tuition partners page
    Then the website link for each tuition partner opens their website in a new tab

  Scenario: tuition partner summaries' phone number link initiates device's calling options and is not empty
    Given a user has arrived on the all quality-assured tuition partners page
    Then the phone number link for each tuition partner initiates their device's calling options and is not empty

  Scenario: tuition partner summaries' email link initiates email client options and is not empty
    Given a user has arrived on the all quality-assured tuition partners page
    Then the email link for each tuition partner initiates their email client options and is not empty

  Scenario: tuition partner details page linked from all quality-assured tuition partners page has 'Back to tuition partners' back link
    Given a user has arrived on the all quality-assured tuition partners page
    When they click on the '4th' tuition partner's name
    Then the back link's text is 'Back to tuition partners'

  Scenario: back link returns to all quality-assured tuition partners page
    Given a user has arrived on the all quality-assured tuition partners page
    When they click on the '21st' tuition partner's name
    And they click 'Back'
    Then they will see the 'All quality-assured tuition partners' page

  Scenario: search by name
    Given a user has arrived on the all quality-assured tuition partners page
    When they search by tuition partner name 'tutor'
    Then they will see the list of quality-assured tuition partners with names containing 'tutor' is in alphabetical order by name
    And they will not see there are no search results for name

  Scenario: search by name displays result count
    Given a user has searched the all quality-assured tuition partners by name 'tutor'
    Then the number of tuition partners displayed matches the displayed count

  Scenario: back link returns to searching all quality-assured tuition partners by name page
    Given a user has searched the all quality-assured tuition partners by name 'tutor'
    When they click on the '2nd' tuition partner's name
    And they click 'Back'
    Then search by tuition partner name is 'tutor'
    And they will see the list of quality-assured tuition partners with names containing 'tutor' is in alphabetical order by name

  Scenario: display there are no search results when no results found for name
    Given a user has arrived on the all quality-assured tuition partners page
    When they search by tuition partner name 'zzz'
    Then they will see there are no search results for 'zzz'
    
Scenario: Logos are displayed for tution partners
    Given a user has arrived on the all quality-assured tuition partners page
    Then logos are shown for tuition partners

Scenario: Logos are not displayed for tution partners
    Given a user is using a 'phone'
    Given a user has arrived on the all quality-assured tuition partners page
    Then logos are not shown for tuition partners

  Scenario: From the full list we can visit each TP details page and see the Type of Tuition details and the pricing in the correct order then is returned to the location of the selected TP on the list
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'Check the full list of quality-assured tuition partners' link
    Then they can visit each TP details page and see the Type of Tuition details and the pricing in the correct order
