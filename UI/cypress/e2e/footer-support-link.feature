Feature: Support is provided to users
  Scenario: The support link is shown to users
    Given a user has started the 'Find a tuition partner' journey
    Then they see the support link at the bottom of the page
    And the support link will open a new email to 'tutoring.support@service.education.gov.uk'
