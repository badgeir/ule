pipeline {
    agent { docker 'python:3.5.1' }
    stages {
        stage('build') {
            steps {
                sh 'python tests/run_all.py'
            }
        }
    }
}
