Feature: User can travel forwards and backwards

Scenario: User can move forward in the application to the end of the journey
   Given a user has started the 'Find a tuition partner' journey
    Then they will be able journey forward to a selected tuition partner page

Scenario: User can travel back to the begining of journey
    Given a user has started the 'Find a tuition partner' journey
        And they will be able journey forward to a selected tuition partner page
    Then they will be journey back to the page the started from

Scenario: User can travel back to the beginning of journey without loss of data
    Given a user has started the 'Find a tuition partner' journey
        And they will be able journey forward to a selected tuition partner page
        And they will be journey back to the page the started from
    Then the key stages are correct in the key stages page
        And the subjects are correct in the subjects page
        And the filter selections are correct in the search results page

Scenario: User can travel back to the begining of journey and edit previous selections
    Given a user has started the 'Find a tuition partner' journey
        And they will be able journey forward to a selected tuition partner page
        And they will be journey back to the page the started from
    When the key stages are edited in the key stages page
        And the subjects are edited in the subjects page after key stage has been edited
    Then the filter selections are correct in the search results page with the edited selections
