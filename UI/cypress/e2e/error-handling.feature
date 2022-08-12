Feature: Error handling

    Scenario: When service runs into error 404 display the ‘page not found’ page
        Given a user has started the 'Find a tuition partner' journey
        When a service page has not been found
        Then the 'page not found' error page is displayed
