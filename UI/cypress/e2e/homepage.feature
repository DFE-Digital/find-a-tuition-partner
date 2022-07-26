Feature: Check home page Content
  Scenario: page title is 'Find a tuition partner'
    Given a user has started the 'Find a tuition partner' journey
    Then the page's title is 'Find a tuition partner'

   Scenario: Check other options tab displayed
    Given a user has started the 'Find a tuition partner' journey
    Then check other tutoring options text is displayed

   Scenario: Check academic mentors link
     Given a user has started the 'Find a tuition partner' journey
     Then the other options academic links to '/academic-mentors'

   Scenario: Check School led tutoring link
     Given a user has started the 'Find a tuition partner' journey
     Then the other options school-led tutoring links to '/school-led-tutoring'
   
