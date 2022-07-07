Feature: User can choose Key Stage and Subject 
  Scenario: User lands on subjects page without associated key stages selected
    Given a user has started the 'Find a tuition partner' journey
    When they manually navigate to the 'Which subjects' page 
    Then they are shown all the subjects under all the keys stages
