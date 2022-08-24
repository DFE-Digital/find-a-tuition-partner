Feature: User is shown search results
  Scenario: page url is '/search-results'
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    Then the page URL ends with '/search-results'

  Scenario: page title is 'Search results'
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    Then the page's title is 'Search results'

  Scenario: user clicks service name
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: Back click returns to subjects page
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'SK1 1EB'
    When they click 'Back'
    Then they will be taken to the 'Which subjects' page
    And they are shown the subjects for 'Key stage 1'
    And they will see 'Key stage 1 English' selected

  Scenario: Back to the start
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'SK1 1EB'
    When they click 'Back'
    And they click 'Back'
    And they click 'Back'
    Then they will be taken to the 'Find a tuition partner' journey start page
    And they will see 'SK1 1EB' entered for the postcode

  Scenario: user does not enter postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' without a postcode
    Then display all correct tuition partners that provide the selected subjects in any location
    And they will not see an error message

  Scenario: user enters an invalid postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    When they enter 'INVALID' as the school's postcode
    And they click 'Continue'
    Then they will see 'Enter a valid postcode' as an error message for the 'postcode'

  Scenario: user enters a postcode in Wales
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    When they enter 'SA1 1DX' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user enters a postcode in Scotland
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    When they enter 'G33 2SQ' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user enters a postcode in Northern Ireland
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    When they enter 'BT47 5QG' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user clicks postcode error
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'invalid'
    When they click on the postcode error
    Then the school's postcode text input is focused

  Scenario: lands on search results page without filter boxes selected
    Given a user has arrived on the 'Search results' page without subjects
    Then display all correct tuition partners that provide the selected subjects in any location

  Scenario: lands on search results with blank URL
    Given a user has arrived on the 'Search results' page without subjects or postcode
    Then display all correct tuition partners in any location
    And they will see all the subjects for 'Key stage 1, Key stage 2, Key stage 3, Key stage 4'

  Scenario: unselects all boxes on the filters 
    Given a user has arrived on the 'Search results' page
    When they enter '' as the school's postcode
    And they clear all the filters
    Then display all correct tuition partners in any location
    And they will see all the subjects for 'Key stage 1, Key stage 2, Key stage 3, Key stage 4'

  Scenario: results default to any tuition type
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    Then they will see the tuition type 'Any' is selected

  Scenario: All key stages are shown
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    Then they will see an expanded subject filter for 'Key stage 1'
    And they will see all the subjects for 'Key stage 1'
    And they will see a collapsed subject filter for 'Key stage 2, Key stage 3, Key stage 4'

  Scenario: Key stage can be collapsed
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    And they click on the option heading for 'Key stage 1'
    And they will see a collapsed subject filter for 'Key stage 1'

  Scenario: Key stage can be expanded
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
    And they click on the option heading for 'Key stage 2'
    And they will see an expanded subject filter for 'Key stage 2'

  Scenario: Subjects are displayed in alphabetical order in  page the filter of 'search results' page
      Given a user has arrived on the 'Search results' page
      Then the subjects in the filter displayed in alphabetical order 

  Scenario: subjects covered by a tuition partner are in alphabetical order in the 'search results' page
      Given a user has arrived on the 'Search results' page
      Then the subjects covered by a tuition partner are in alphabetical order 

  Scenario: Results summary is shown 
    Given a user has arrived on the 'Search results' page for 'Key stage 1 English' for postcode 'SK1 1EB'
    Then they will see the results summary for 'Stockport'

  Scenario: Local Education Authority name is displayed for postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 2 Maths' for postcode 'HP4 3LG'
    Then they will see the results summary for 'Hertfordshire'

  Scenario: Results page  contact us back link redirects to right page
    Given a user has arrived on the 'Search results' page for 'Key stage 2 Maths' for postcode 'HP4 3LG'
    Then they will click the contact us link
    And the user clicks on back link
    Then they will see the results summary for 'Hertfordshire'

  
  