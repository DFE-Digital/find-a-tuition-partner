Feature: Security headers
    Scenario: Security header X-Frame-Options added to page
        Given a user has started the 'Find a tuition partner' journey
        Then header 'X-Frame-Options' is added to the displayed page

    Scenario: Security header Content-Security-Policy added to page
        Given a user has started the 'Find a tuition partner' journey
        Then header 'Content-Security-Policy' is added to the displayed page

    Scenario: Security header X-Xss-Protection added to page
        Given a user has started the 'Find a tuition partner' journey
        Then header 'X-XSS-Protection' is added to the displayed page

    Scenario: Security header X-Content-Type-Options added to page
        Given a user has started the 'Find a tuition partner' journey
        Then header 'X-Content-Type-Options' is added to the displayed page