Feature: User enters postcode to begin search
  Scenario: page url is '/'
    Given a user has started the 'Find a tuition partner' journey
    Then the page URL ends with '/'

  Scenario: page title is 'Find a tuition partner'
    Given a user has started the 'Find a tuition partner' journey
    Then the page's title is 'Find a tuition partner'

  Scenario: user clicks service name
    Given a user has started the 'Find a tuition partner' journey
    When they click the 'Find a tuition partner' service name link
    Then they will be taken to the 'Find a tuition partner' journey start page

  Scenario: no back link on the 'Find a tuition partner' journey start page
    Given a user has started the 'Find a tuition partner' journey
    Then the 'Back' link is not displayed

  Scenario: quality assured tuition partner details is initially hidden
    Given a user has started the 'Find a tuition partner' journey
    Then the quality assured tuition partner details are hidden

  Scenario: user clicks quality assured tuition partner details summary
    Given a user has started the 'Find a tuition partner' journey
    When they click 'What is a quality-assured tuition partner?'
    Then the quality assured tuition partner details are shown

  Scenario: user does not enter postcode
    Given a user has started the 'Find a tuition partner' journey
    When they click 'Continue'
    Then they will see 'Enter a postcode' as an error message for the 'postcode'

  Scenario: user enters an invalid postcode
    Given a user has started the 'Find a tuition partner' journey
    When they enter 'INVALID' as the school's postcode
    And they click 'Continue'
    Then they will see 'Enter a valid postcode' as an error message for the 'postcode'

  Scenario: user enters a postcode in Wales
    Given a user has started the 'Find a tuition partner' journey
    When they enter 'SA1 1DX' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user enters a postcode in Scotland
    Given a user has started the 'Find a tuition partner' journey
    When they enter 'G33 2SQ' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user enters a postcode in Northern Ireland
    Given a user has started the 'Find a tuition partner' journey
    When they enter 'BT47 5QG' as the school's postcode
    And they click 'Continue'
    Then they will see 'This service covers England only' as an error message for the 'postcode'

  Scenario: user clicks postcode error
    Given a user has tried to continue without entering a postcode
    When they click on the postcode error
    Then the school's postcode text input is focused

  Scenario: user enters a valid postcode
    Given a user has started the 'Find a tuition partner' journey
    When they enter 'SK1 1EB' as the school's postcode
    And they click 'Continue'
    Then they will be taken to the 'Which key stages' page
