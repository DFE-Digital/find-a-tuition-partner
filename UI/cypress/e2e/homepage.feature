Feature: Homepage
  Scenario: visting the service homepage
    When a user visits the homepage
    Then the heading should say 'The National Tutoring Programme'

  Scenario: starting the journey
    Given a user has arrived on the homepage
    When they click 'Start now'
    Then they will be taken to the 'Compare national tutoring options' page
