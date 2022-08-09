Feature: Support is provided to users

  Scenario: The support link is shown to users
    Given a user has started the 'Find a tuition partner' journey
    Then they see the support link at the bottom of the page
    And the support link will open a new email to 'tutoring.support@service.education.gov.uk'

  Scenario: Accessibility link is accessible in footer
    Given Any service page has been selected
    Then display the following links in the footer, report a problem with email

  Scenario: Privacy policy link is accessible in footer
    Given Any service page has been selected
    When 
    Then 