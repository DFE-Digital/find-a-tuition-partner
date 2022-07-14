Feature: User is shown search results
  Scenario: page url is '/search-results'
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    Then the page URL ends with '/search-results'

  Scenario: page title is 'Search results'
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    Then the page's title is 'Search results'

  Scenario: user clicks service name
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: user does not enter postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 1' without a postcode
    When they click 'Continue'
    Then they will see 'Enter a postcode' as an error message for the 'postcode'

  Scenario: user enters an invalid postcode
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    When they enter 'INVALID' as the school's postcode
    And they click 'Continue'
    Then they will see 'Enter a valid postcode' as an error message for the 'postcode'

  Scenario: user enters a postcode in Wales
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    When they enter 'SA1 1DX' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user enters a postcode in Scotland
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    When they enter 'G33 2SQ' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user enters a postcode in Northern Ireland
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    When they enter 'BT47 5QG' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user clicks postcode error
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    When they click on the postcode error
    Then the school's postcode text input is focused

  Scenario: results default to any tuition type
    Given a user has arrived on the 'Search results' page for 'Key stage 1'
    Then they will see the tuition type 'Any' is selected