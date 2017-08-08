pipeline {
    agent { docker 'python:3.5.1' }
    stages {
        stage('build') {
            steps {
                bat 'C:/Anaconda3/python tests/run_all.py'
            }
        }
    }
}
