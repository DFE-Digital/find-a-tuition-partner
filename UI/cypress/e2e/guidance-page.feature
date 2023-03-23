Feature: Guidance Page at the start of the enquiry builder

Scenario: Guidance page is at the begininng of the enquiry builder
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When they click 'Start now' button
And the user will navigate to the guidance page
And they click 'Continue' on enquiry
Then user is redirected to the enter email address page

Scenario: Clicking back on guidance page to navigate to search results page
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When they click 'Start now' button
And they click 'Back'
Then they will be taken to the 'Search Results' page

Scenario: Guidance page has expected structure
Given a user has arrived on the 'Search results' page for 'Key stage 1 English'
When they click 'Start now' button
Then the page's title is 'Send an enquiry to tuition partners'

Scenario: user clicks service name on guidance page
Given the user will navigate to the guidance page
When they click the 'Find a tuition partner' service name link
Then they will be taken to the 'Find a tuition partner' journey start page