Feature: Login
    As a user I want to be able to log into the Magine API

Scenario: Log in with valid credentials
    Given I have entered the following login data:
    | UserName                    | Password |
    | maginemobdevtest@magine.com | magine   |
    When I log in
    Then Magine API is called with expected user name and password
    And I will be logged in

Scenario: Log in without supplying user name
    Given I have not entered user name
    When I log in
    Then I will not be logged in
    And I receive an error stating "User name is mandatory"

Scenario: Log in without supplying password
    Given I have not entered password
    When I log in
    Then I will not be logged in
    And I receive an error stating "Password is mandatory"

Scenario: Log in with invalid credentials
    Given I have entered invalid user name or password
    When I log in
    Then Magine API is called with expected user name and password
    And I will not be logged in
    And I receive an error stating "The user name/password you entered is invalid"