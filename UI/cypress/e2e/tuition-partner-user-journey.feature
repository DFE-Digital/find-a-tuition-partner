Feature: User can travel forwards and backwards

Scenario: User can move forward in the application to the end of the journey
   Given a user has started the 'Find a tuition partner' journey
    Then they will be able journey forward to a selected tuition partner page

Scenario: User can travel back to the begining of journey
    Given a user has started the 'Find a tuition partner' journey
        And they will be able journey forward to a selected tuition partner page
    Then they will be journey back to the page the started from


