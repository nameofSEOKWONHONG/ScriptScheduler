# nameing example
## [MODE].[TIME].[ROLE_NAME].[EXTENSION]
## MODE
### CN : CONTINUE
### DL : DAILY (TIME)
### ON : ONCE (delete script file after once execute)
## TIME (4 letters)
### C : 0
### D : 1200
## ROLE_NAME : filename 
## EXTENSION : py, csx

from selenium import webdriver

from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

driver = webdriver.Chrome()
driver.get(url='[url]')

try:
    WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.CLASS_NAME, 'login')))

    login = driver.find_element(By.CLASS_NAME, "login")
    pw = driver.find_element(By.ID, "mb_password")
    login_btn = driver.find_element(By.CLASS_NAME, "bigLoginBtn")

    login.send_keys("id")
    pw.send_keys("pw")
    login_btn.click()

    WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.CLASS_NAME, "change_profile_image_button")))
finally:
    driver.quit()